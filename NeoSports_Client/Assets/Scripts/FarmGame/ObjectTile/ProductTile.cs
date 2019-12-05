﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public struct ProductData
	{
		public int type;
		public string name;
		public int price;
		public int grownHour;
		public int grownMin;
		public string lessGrownSprite;
		public string fullGrownSprite;
	}

	public class ProductTile : ObjectTile
	{
		enum Echild
		{
			LessGrownSprite,
			FullGrownSprite
		}

		public struct LoadData
		{
			public Point point;
			public int productType;
			public DateTime plantingTime;
			public DateTime harvestTime;
		}

		ProductData productData;

		DateTime _plantingTime;
		DateTime _harvestTime;
		bool _canHarvest;

		public void PlantProduct(Point point, int type)
		{
			productData = MapData.Instance.ProductDatas[type];
			base.point = point;

			TimeSpan grownTime = new TimeSpan(productData.grownHour, productData.grownMin, 0);
			_plantingTime = DateTime.Now;
			_harvestTime = _plantingTime.Add(grownTime);
			_canHarvest = false;

			StartCoroutine(CheckCanHarvest());
			SetPosition();
			InitSprite();
			SetSprite();
		}

		public void LoadTileData(LoadData data)
		{
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
				StartCoroutine(CheckCanHarvest());
			}

			SetPosition();
			InitSprite();
			SetSprite();
		}

		void SetPosition()
		{
			Vector3 position = Vector3.zero;

			position.x = MapData.TileSize * point.X;
			position.y = MapData.TileSize * point.Y;

			transform.localPosition = position;
		}

		void InitSprite()
		{
			SpriteRenderer renderer;
			string spriteName;

			renderer = transform.GetChild((int)Echild.LessGrownSprite).GetComponent<SpriteRenderer>();
			spriteName = productData.lessGrownSprite;
			renderer.sprite = ResourceManager.Instance.GetFarmAtlas(spriteName);

			renderer = transform.GetChild((int)Echild.FullGrownSprite).GetComponent<SpriteRenderer>();
			spriteName = productData.fullGrownSprite;
			renderer.sprite = ResourceManager.Instance.GetFarmAtlas(spriteName);
		}

		void SetSprite()
		{
			if (_canHarvest)
			{
				transform.GetChild((int)Echild.FullGrownSprite).gameObject.SetActive(true);
			}
			else
			{
				transform.GetChild((int)Echild.FullGrownSprite).gameObject.SetActive(false);
			}
		}

		IEnumerator CheckCanHarvest()
		{
			while (true)
			{
				if (_harvestTime < DateTime.Now)
				{
					_canHarvest = true;
					SetSprite();
					Debug.Log("Harvest Time!");
					yield break;
				}
				else
				{
					Debug.Log(DateTime.Now - _harvestTime);
					yield return new WaitForSeconds(10.0f);
				}
			}
		}
	}
}
