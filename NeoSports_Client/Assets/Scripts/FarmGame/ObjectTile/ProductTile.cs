using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;

public struct ProductData
{
	public EProduct type;
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
		public EProduct productType;
		public DateTime plantingTime;
		public DateTime harvestTime;
	}

	ProductData productData;

	DateTime _plantingTime;
	DateTime _harvestTime;
	bool _canHarvest;

	public SpriteAtlas farmLandTileAtlas;

	public void PlantProduct(Point pt, EProduct type)
	{
		productData = MapData.Instance.ProductDatas[type];
		point = pt;

		TimeSpan grownTime = new TimeSpan(productData.grownHour, productData.grownMin, 0);
		_plantingTime = DateTime.Now;
		_harvestTime = _plantingTime.Add(grownTime);
		_canHarvest = false;

		StartCoroutine(CheckCanHarvest());
		SetPosition(pt);
		InitSprite();
		SetSprite();
	}

	public void LoadDataProduct(LoadData data)
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

		SetPosition(data.point);
		InitSprite();
		SetSprite();
	}

	void SetPosition(Point pt)
	{
		Vector3 position = Vector3.zero;

		position.x = MapData.TileSize * pt.X;
		position.y = MapData.TileSize * pt.Y;

		transform.localPosition = position;
	}

	void InitSprite()
	{
		SpriteRenderer renderer;
		string spriteName;

		renderer = transform.GetChild((int)Echild.LessGrownSprite).GetComponent<SpriteRenderer>();
		spriteName = productData.lessGrownSprite;
		renderer.sprite = farmLandTileAtlas.GetSprite(spriteName);

		renderer = transform.GetChild((int)Echild.FullGrownSprite).GetComponent<SpriteRenderer>();
		spriteName = productData.fullGrownSprite;
		renderer.sprite = farmLandTileAtlas.GetSprite(spriteName);
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
