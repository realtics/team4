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

		public struct SaveData
		{
			public Point point;
			public int decorationType;
		}

		DecorationData decorationData;

		public void DeployTile(Point point, int type)
		{
			tileType = ETileType.Decoration;
			decorationData = MapData.Instance.DecorationDatas[type];
			base.point = point;

			SetPosition();
			InitSprite();
		}

		public void LoadSaveData(SaveData data)
		{
			tileType = ETileType.Decoration;
			decorationData = MapData.Instance.DecorationDatas[data.decorationType];
			point = data.point;

			SetPosition();
			InitSprite();
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

			renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
			spriteName = decorationData.sprite;
			renderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
		}

	}

}
