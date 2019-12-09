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

		public GameObject lessGrownSpriteObject;
		public GameObject fullGrownSpriteObject;
		public GameObject harvestTimeTextObject;

		ProductData productData;

		DateTime _plantingTime;
		DateTime _harvestTime;
		bool _canHarvest;

		TextMeshPro _harvestTimeTextMesh;

		private void Awake()
		{
			tileType = ETileType.Harvest;
			_harvestTimeTextMesh = harvestTimeTextObject.GetComponent<TextMeshPro>();
		}

		public void PlantProduct(Point point, int type)
		{
			productData = MapData.Instance.ProductDatas[type];
			base.point = point;

			_plantingTime = DateTime.Now;
			_harvestTime = _plantingTime.AddMinutes(productData.grownTime);
			_canHarvest = false;

			StartCoroutine(UpdateHarvestTime());
			UpdatePosition();
			UpdateSprite();
			UpdateGrownSpriteActive();
		}

		public void LoadSaveData(SaveData data)
		{
			tileType = ETileType.Harvest;
			productData = MapData.Instance.ProductDatas[data.productType];
			point = data.point;

			_plantingTime = data.plantingTime;
			_harvestTime = data.harvestTime;

			if (_harvestTime < DateTime.Now)
			{
				_canHarvest = true;
			}
			else
			{
				_canHarvest = false;
				StartCoroutine(UpdateHarvestTime());
			}

			UpdatePosition();
			UpdateSprite();
			UpdateGrownSpriteActive();
		}

		public SaveData MakeSaveData()
		{
			SaveData data;
			data.point = point;
			data.productType = productData.type;
			data.plantingTime = _plantingTime;
			data.harvestTime = _harvestTime;

			return data;
		}

		void UpdatePosition()
		{
			Vector3 position = Vector3.zero;

			position.x = MapData.TileSize * point.X;
			position.y = MapData.TileSize * point.Y;

			transform.localPosition = position;
		}

		void UpdateSprite()
		{
			SpriteRenderer renderer;
			string spriteName;

			renderer = lessGrownSpriteObject.GetComponent<SpriteRenderer>();
			spriteName = productData.lessGrownSprite;
			renderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);

			renderer = fullGrownSpriteObject.GetComponent<SpriteRenderer>();
			spriteName = productData.fullGrownSprite;
			renderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
		}

		void UpdateGrownSpriteActive()
		{
			if (_canHarvest)
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
					_canHarvest = true;
					UpdateGrownSpriteActive();
					Debug.Log("Harvest Time!");
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
