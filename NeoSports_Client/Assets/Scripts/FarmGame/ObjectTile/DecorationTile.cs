using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public struct DecorationData
	{
		public int type;
		public string name;
		public int price;
		public string sprite;
	}

	public class DecorationTile : ObjectTile
	{

		public struct LoadData
		{
			public Point point;
			public int decorationType;
		}

		DecorationData decorationData;

		public void DeployTile(Point point, int type)
		{
			decorationData = MapData.Instance.DecorationDatas[type];
			base.point = point;

			InitSprite();
		}

		public void LoadTileData(LoadData data)
		{
			decorationData = MapData.Instance.DecorationDatas[data.decorationType];
			point = data.point;

			InitSprite();
		}

		void InitSprite()
		{
			SpriteRenderer renderer;
			string spriteName;

			renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
			spriteName = decorationData.sprite;
			renderer.sprite = ResourceManager.Instance.GetFarmAtlas(spriteName);
		}

	}

}
