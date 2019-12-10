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

		DecorationData _decorationData;

		private void Awake()
		{
			_tileType = ETileType.Decoration;
		}

		public void DeployTile(Point point, int type)
		{
			_decorationData = MapData.Instance.DecorationDataDic[type];
			_point = point;

			UpdatePosition();
			UpdateSprite();
			
		}

		public void SetSaveData(SaveData data)
		{
			_decorationData = MapData.Instance.DecorationDataDic[data.decorationType];
			_point = data.point;

			UpdatePosition();
			UpdateSprite();
		}

		public SaveData GetSaveData()
		{
			SaveData data;
			data.point = _point;
			data.decorationType = _decorationData.type;

			return data;
		}

		void UpdatePosition()
		{
			Vector3 position = Vector3.zero;

			position.x = MapData.TileSize * _point.X;
			position.y = MapData.TileSize * _point.Y;

			transform.localPosition = position;
		}

		void UpdateSprite()
		{
			SpriteRenderer renderer;
			string spriteName;

			renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
			spriteName = _decorationData.sprite;
			renderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
		}

	}

}
