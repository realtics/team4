#pragma warning disable CS0649
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
			public int type;
		}

		[SerializeField]
		SpriteRenderer spriteRenderer;

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
			_decorationData = MapData.Instance.DecorationDataDic[data.type];
			_point = data.point;

			UpdatePosition();
			UpdateSprite();
		}

		public SaveData GetSaveData()
		{
			SaveData data;
			data.point = _point;
			data.type = _decorationData.type;

			return data;
		}

		void UpdateSprite()
		{
			string spriteName = _decorationData.sprite;

			spriteRenderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
		}

	}

}
