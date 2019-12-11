using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;

namespace FarmGame
{
	public struct LandData
	{
		public string type;
		public int variationCount;
	}

	public class LandTile : MonoBehaviour
	{
		public struct SaveData
		{
			public Point point;
			public string type;
			public int variationIndex;
		}

		public const string BadlandType = "badland";
		public const string GrassType = "grass";
		public const string CultivateType = "cultivate";

		public GameObject highlightSpriteObject;

		Point _point;
		LandData _landData;
		int _variationIndex;

		#region Property
		public bool Highlight {
			set { highlightSpriteObject.SetActive(value); }
			get { return highlightSpriteObject.activeInHierarchy; }
		}

		public Point MapPoint {
			get { return _point; }
		}

		public string Type {
			get { return _landData.type; }
		}
		#endregion

		public void CreateLand(Point point, LandData data)
		{
			_point = point;
			_landData = data;
			_variationIndex = Random.Range(0, data.variationCount);

			UpdatePosition();
			UpdateSprite();
		}

		public void SetSaveData(SaveData data)
		{
			_point = data.point;
			_landData = MapData.Instance.LandDataDic[data.type];
			_variationIndex = data.variationIndex;

			UpdatePosition();
			UpdateSprite();
		}

		public SaveData GetSaveData()
		{
			SaveData data;
			data.point = _point;
			data.type = _landData.type;
			data.variationIndex = _variationIndex;

			return data;
		}

		public void ChangeType(LandData data)
		{
			_landData = data;
			_variationIndex = Random.Range(0, data.variationCount);

			UpdateSprite();
			MapData.Instance.WriteSaveData(MapData.ESaveType.Land);
		}

		void UpdatePosition()
		{
			Vector3 position = Vector3.zero;
			position.x = _point.X * MapData.TileSize;
			position.y = _point.Y * MapData.TileSize;

			transform.localPosition = position;
		}

		void UpdateSprite()
		{
			string spriteName = _landData.type + "_" + _variationIndex.ToString();
			Sprite tileSprite = ResourceManager.Instance.GetFarmSprite(spriteName);
			GetComponent<SpriteRenderer>().sprite = tileSprite;
		}

	}

}
