using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public class RoadTile : ObjectTile
	{
		public enum EMaterial
		{
			Wire,
			Fence
		}

		public enum EType
		{
			Default,
			Lamp,
			Trash,
			Corner
		}

		public struct TileData
		{
			public Point point;
			public EMaterial material;
			public EType type;
		}

		const float EdgeTopRotation = 0;
		const float EdgeLeftRotation = 90;
		const float EdgeBottomRotation = 180;
		const float EdgeRightRotation = 270;

		const float CornerTopRightRotation = 0;
		const float CornerTopLeftRotation = 90;
		const float CornerBottomLeftRotation = 180;
		const float CornerBottomRightRotation = 270;

		EMaterial material;
		EType type;

		#region Property
		public EMaterial Material {
			get { return material; }
			set {
				material = value;
				SetSprite();
			}
		}

		public EType Type {
			get { return type; }
			set {
				type = value;
				SetSprite();
			}
		}
		#endregion

		public void SetData(TileData data)
		{
			tileType = ETileType.Road;
			material = data.material;
			type = data.type;
			name = data.point.X.ToString() + "_" + data.point.Y.ToString() + "_RoadTile";

			SetPosition(data.point);
			SetRotation(data.point);

			SetSprite();
		}

		void SetPosition(Point pt)
		{
			Vector3 position = Vector3.zero;

			position.x = pt.X * MapData.TileSize;
			position.y = pt.Y * MapData.TileSize;

			transform.localPosition = position;
		}

		void SetRotation(Point pt)
		{
			if (IsCornerTile(pt))
			{
				SetCornerRotation(pt);
			}
			else
			{
				SetEdgeRotation(pt);
			}
		}

		bool IsCornerTile(Point pt)
		{
			if (pt.X == 0 || pt.X == MapData.MapWidth - 1)
			{
				if (pt.Y == 0 || pt.Y == MapData.MapHeight - 1)
				{
					type = EType.Corner;
					return true;
				}
			}

			return false;
		}

		void SetCornerRotation(Point pt)
		{
			Vector3 rotation = Vector3.zero;

			if (pt.X == 0)
			{
				if (pt.Y == 0)
				{
					rotation.z = CornerBottomLeftRotation;
				}
				else if (pt.Y == MapData.MapHeight - 1)
				{
					rotation.z = CornerTopLeftRotation;
				}
			}
			else if (pt.X == MapData.MapWidth - 1)
			{
				if (pt.Y == 0)
				{
					rotation.z = CornerBottomRightRotation;
				}
				else if (pt.Y == MapData.MapHeight - 1)
				{
					rotation.z = CornerTopRightRotation;
				}
			}

			Quaternion q = Quaternion.identity;
			q.eulerAngles = rotation;
			transform.localRotation = q;

		}

		void SetEdgeRotation(Point pt)
		{
			Vector3 rotation = Vector3.zero;

			if (pt.X == 0)
			{
				rotation.z = EdgeLeftRotation;
			}
			else if (pt.X == MapData.MapWidth - 1)
			{
				rotation.z = EdgeRightRotation;
			}
			else if (pt.Y == 0)
			{
				rotation.z = EdgeBottomRotation;
			}
			else if (pt.Y == MapData.MapHeight - 1)
			{
				rotation.z = EdgeTopRotation;
			}

			Quaternion q = Quaternion.identity;
			q.eulerAngles = rotation;
			transform.localRotation = q;
		}

		void SetSprite()
		{
			string spriteName = string.Empty;
			switch (material)
			{
				case EMaterial.Wire:
					spriteName += "wire";
					break;
				case EMaterial.Fence:
					spriteName += "fence";
					break;
			}

			spriteName += "_";

			switch (type)
			{
				case EType.Default:
					spriteName += "default";
					break;
				case EType.Lamp:
					spriteName += "lamp";
					break;
				case EType.Trash:
					spriteName += "Trash";
					break;
				case EType.Corner:
					spriteName += "corner";
					break;
			}

			Sprite sprite = ResourceManager.Instance.GetFarmAtlas(spriteName);
			GetComponent<SpriteRenderer>().sprite = sprite;
		}
	}

}
