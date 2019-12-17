using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public struct GarbageData
	{
		public int type;
		public string name;
		public int removeCost;
		public string sprite;
	}

	public class GarbageTile : ObjectTile
	{
		public struct SaveData
		{
			public Point point;
			public int type;
		}

		[SerializeField]
		SpriteRenderer spriteRenderer;

		GarbageData _garbageData;

		private void Awake()
		{
			_tileType = ETileType.Garbage;
		}

		public void DeployTile(Point point, int type)
		{
			_garbageData = MapData.Instance.GarbageDataDic[type];
			_point = point;

			UpdatePosition();
			UpdateSprite();
		}

		public void SetSaveData(SaveData data)
		{
			_garbageData = MapData.Instance.GarbageDataDic[data.type];
			_point = data.point;

			UpdatePosition();
			UpdateSprite();
		}

		public SaveData GetSaveData()
		{
			SaveData data;
			data.point = _point;
			data.type = _garbageData.type;

			return data;
		}

		public GarbageData GetGarbageData()
		{
			return _garbageData;
		}

		void UpdateSprite()
		{
			string spriteName = _garbageData.sprite; 

			spriteRenderer.sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
		}

	}
}


