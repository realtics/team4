using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public struct RoadData
	{
		public Point point;
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

		public void SetData(RoadData data)
		{
			tileType = ETileType.Road;
			_roadData = data;
			name = data.point.X.ToString() + "_" + data.point.Y.ToString() + "_RoadTile";

			UpdatePosition(data.point);
			UpdateRotation(data.point);
			UpdateSprite();
		}

		public void LoadSaveData(SaveData data)
		{

		}

		public SaveData MakeSaveData()
		{
			SaveData data = new SaveData();
			return data;
		}

		void UpdatePosition(Point point)
		{
			Vector3 position = Vector3.zero;

			position.x = point.X * MapData.TileSize;
			position.y = point.Y * MapData.TileSize;

			transform.localPosition = position;
		}

		void UpdateRotation(Point point)
		{
			if (IsCornerTile(point))
			{
				UpdateCornerRotation(point);
			}
			else
			{
				UpdateEdgeRotation(point);
			}
		}

		bool IsCornerTile(Point point)
		{
			if (point.X == 0 || point.X == MapData.MapWidth - 1)
			{
				if (point.Y == 0 || point.Y == MapData.MapHeight - 1)
				{
					_roadData.type = "corner";
					return true;
				}
			}

			return false;
		}

		void UpdateCornerRotation(Point point)
		{
			Vector3 rotation = Vector3.zero;

			if (point.X == 0)
			{
				if (point.Y == 0)
				{
					rotation.z = CornerBottomLeftRotation;
				}
				else if (point.Y == MapData.MapHeight - 1)
				{
					rotation.z = CornerTopLeftRotation;
				}
			}
			else if (point.X == MapData.MapWidth - 1)
			{
				if (point.Y == 0)
				{
					rotation.z = CornerBottomRightRotation;
				}
				else if (point.Y == MapData.MapHeight - 1)
				{
					rotation.z = CornerTopRightRotation;
				}
			}

			Quaternion q = Quaternion.identity;
			q.eulerAngles = rotation;
			transform.localRotation = q;
		}

		void UpdateEdgeRotation(Point point)
		{
			Vector3 rotation = Vector3.zero;

			if (point.X == 0)
			{
				rotation.z = EdgeLeftRotation;
			}
			else if (point.X == MapData.MapWidth - 1)
			{
				rotation.z = EdgeRightRotation;
			}
			else if (point.Y == 0)
			{
				rotation.z = EdgeBottomRotation;
			}
			else if (point.Y == MapData.MapHeight - 1)
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
