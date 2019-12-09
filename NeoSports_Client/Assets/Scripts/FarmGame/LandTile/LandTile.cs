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
		#endregion

		public void CreateLand(Point point, LandData data)
		{
			_point = point;

			Vector3 position = Vector3.zero;
			position.x = point.X * MapData.TileSize;
			position.y = point.Y * MapData.TileSize;

			_landData = data;

			SetSprite();
		}

		public void ChangeType(LandData data)
		{
			_landData = data;
			_variationIndex = Random.Range(0, data.variationCount);

			SetSprite();
		}

		void SetSprite()
		{
			string spriteName = _landData.type + "_" + _variationIndex.ToString();
			Sprite tileSprite = ResourceManager.Instance.GetFarmSprite(spriteName);
			GetComponent<SpriteRenderer>().sprite = tileSprite;
		}

	}

}
