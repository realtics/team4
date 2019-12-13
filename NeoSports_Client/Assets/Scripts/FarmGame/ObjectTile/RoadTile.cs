using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public struct RoadData
	{
		public string material;
		public string type;
	}

	public class RoadTile : ObjectTile
	{

		public struct SaveData
		{
			public Point point;
			public string material;
			public string type;
		}

		const float EdgeTopRotation = 0;
		const float EdgeLeftRotation = 90;
		const float EdgeBottomRotation = 180;
		const float EdgeRightRotation = 270;

		const float CornerTopRightRotation = 0;
		const float CornerTopLeftRotation = 90;
		const float CornerBottomLeftRotation = 180;
		const float CornerBottomRightRotation = 270;

		RoadData _roadData;

		private void Awake()
		{
			_tileType = ETileType.Road;
		}

		public void DeployRoad(Point point, RoadData data)
		{
			_point = point;
			_roadData = data;
			name = _point.X.ToString() + "_" + _point.Y.ToString() + "_RoadTile";

			UpdatePosition();
			UpdateRotation();
			UpdateSprite();
		}

		public void SetSaveData(SaveData data)
		{
			_point = data.point;
			_roadData.material = data.material;
			_roadData.type = data.type;
			name = _point.X.ToString() + "_" + _point.Y.ToString() + "_RoadTile";

			UpdatePosition();
			UpdateRotation();
			UpdateSprite();
		}

		public SaveData GetSaveData()
		{
			SaveData data = new SaveData();
			data.point = _point;
			data.material = _roadData.material;
			data.type = _roadData.type;
			return data;
		}

		void UpdateRotation()
		{
			if (IsCornerTile())
			{
				UpdateCornerRotation();
			}
			else
			{
				UpdateEdgeRotation();
			}
		}

		bool IsCornerTile()
		{
			if (_point.X == 0 || _point.X == MapData.MapWidth - 1)
			{
				if (_point.Y == 0 || _point.Y == MapData.MapHeight - 1)
				{
					_roadData.type = "corner";
					return true;
				}
			}

			return false;
		}

		void UpdateCornerRotation()
		{
			Vector3 rotation = Vector3.zero;

			if (_point.X == 0)
			{
				if (_point.Y == 0)
				{
					rotation.z = CornerBottomLeftRotation;
				}
				else if (_point.Y == MapData.MapHeight - 1)
				{
					rotation.z = CornerTopLeftRotation;
				}
			}
			else if (_point.X == MapData.MapWidth - 1)
			{
				if (_point.Y == 0)
				{
					rotation.z = CornerBottomRightRotation;
				}
				else if (_point.Y == MapData.MapHeight - 1)
				{
					rotation.z = CornerTopRightRotation;
				}
			}

			Quaternion q = Quaternion.identity;
			q.eulerAngles = rotation;
			transform.localRotation = q;
		}

		void UpdateEdgeRotation()
		{
			Vector3 rotation = Vector3.zero;

			if (_point.X == 0)
			{
				rotation.z = EdgeLeftRotation;
			}
			else if (_point.X == MapData.MapWidth - 1)
			{
				rotation.z = EdgeRightRotation;
			}
			else if (_point.Y == 0)
			{
				rotation.z = EdgeBottomRotation;
			}
			else if (_point.Y == MapData.MapHeight - 1)
			{
				rotation.z = EdgeTopRotation;
			}

			Quaternion q = Quaternion.identity;
			q.eulerAngles = rotation;
			transform.localRotation = q;
		}

		void UpdateSprite()
		{
			string spriteName = _roadData.material + "_" + _roadData.type;
			Sprite sprite = ResourceManager.Instance.GetFarmSprite(spriteName);
			GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}

}
