#pragma warning disable CS0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

namespace FarmGame
{
	public struct ProductData
	{
		public int type;
		public string name;
		public int price;
		public int grownTime;
		public string lessGrownSprite;
		public string fullGrownSprite;
	}

	public class ProductTile : ObjectTile
	{

		public struct SaveData
		{
			public Point point;
			public int productType;
			public DateTime plantingTime;
			public DateTime harvestTime;
		}

		[SerializeField]
		GameObject lessGrownSpriteObject;
		[SerializeField]
		GameObject fullGrownSpriteObject;
		[SerializeField]
		GameObject harvestTimeTextObject;

		TextMeshPro _harvestTimeTextMesh;

		ProductData _productData;
		DateTime _plantingTime;
		DateTime _harvestTime;
		string _landType;

		public bool CanHarvest { get; private set; }
		public ProductData ProductData {
			get { return _productData; }
		}

		private void Awake()
		{
			_tileType = ETileType.Product;
			_harvestTimeTextMesh = harvestTimeTextObject.GetComponent<TextMeshPro>();
		}

		public void PlantProduct(Point point, int productType, string landType)
		{
			_point = point;
			_productData = MapData.Instance.ProductDataDic[productType];
			_landType = landType;

			_plantingTime = DateTime.Now;
			CanHarvest = false;

			CalcHarvestTime();
			StartCoroutine(UpdateHarvestTime());
			UpdatePosition();
			UpdateSprite();
			UpdateGrownSpriteActive();
		}

		public void UpdateLandType(string landType)
		{
			_landType = landType;
			CalcHarvestTime();
		}

		public void CalcHarvestTime()
		{
			float scale;
			switch (_landType)
			{
				case LandTile.BadlandType:
					scale = LandTile.BadlandGrownSpeedScale;
					break;
				case LandTile.GrassType:
					scale = LandTile.GrassGrownSpeedScale;
					break;
				case LandTile.CultivateType:
					scale = LandTile.CultivateGrownSpeedScale;
					break;
				default:
					scale = 1.0f;
					break;
			}
			_harvestTime = _plantingTime.AddMinutes(_productData.grownTime * scale);
		}

		public void HarvestProduct()
		{
			if (CanHarvest)
			{
				ResourceManager.Instance.AddProductResource(_productData.type, 1);
				_plantingTime = DateTime.Now;
				CanHarvest = false;

				CalcHarvestTime();
				StartCoroutine(UpdateHarvestTime());
				UpdateGrownSpriteActive();
				MapData.Instance.WriteSaveData(MapData.ESaveType.Product);
			}
		}

		public void SetSaveData(SaveData data, string landType)
		{
			_tileType = ETileType.Product;
			_point = data.point;
			_productData = MapData.Instance.ProductDataDic[data.productType];
			_landType = landType;

			_plantingTime = data.plantingTime;
			_harvestTime = data.harvestTime;

			if (_harvestTime < DateTime.Now)
			{
				CanHarvest = true;
			}
			else
			{
				CanHarvest = false;
				StartCoroutine(UpdateHarvestTime());
			}

			UpdatePosition();
			UpdateSprite();
			UpdateGrownSpriteActive();
		}

		public SaveData GetSaveData()
		{
			SaveData data;
			data.point = _point;
			data.productType = _productData.type;
			data.plantingTime = _plantingTime;
			data.harvestTime = _harvestTime;

			return data;
		}

		void UpdateSprite()
		{
			SpriteRenderer renderer;
			string spriteName;

			renderer = lessGrownSpriteObject.GetComponent<SpriteRenderer>();
			spriteName = _productData.lessGrownSprite;
			renderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);

			renderer = fullGrownSpriteObject.GetComponent<SpriteRenderer>();
			spriteName = _productData.fullGrownSprite;
			renderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
		}

		void UpdateGrownSpriteActive()
		{
			if (CanHarvest)
			{
				fullGrownSpriteObject.SetActive(true);
				harvestTimeTextObject.SetActive(false);
			}
			else
			{
				fullGrownSpriteObject.SetActive(false);
				harvestTimeTextObject.SetActive(true);
			}
		}

		IEnumerator UpdateHarvestTime()
		{
			while (true)
			{
				if (_harvestTime < DateTime.Now)
				{
					CanHarvest = true;
					UpdateGrownSpriteActive();
					yield break;
				}
				else
				{
					TimeSpan remainTime = _harvestTime - DateTime.Now;
					_harvestTimeTextMesh.text = remainTime.ToString(@"hh\:mm\:ss");
					yield return new WaitForSeconds(1.0f);
				}
			}
		}
	}
}
