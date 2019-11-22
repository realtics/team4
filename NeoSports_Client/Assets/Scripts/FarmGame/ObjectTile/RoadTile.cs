using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;

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

	public SpriteAtlas farmLandTileAtlas;

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
		SetTransform(data.point);
		name = data.point.X.ToString() + "_" + data.point.Y.ToString() + "_RoadTile";
		SetSprite();
	}

	void SetTransform(Point pt)
	{
		Vector3 position = Vector3.zero;
		Quaternion rotation = new Quaternion();

		position.x = pt.X * MapData.TileSize;
		position.y = pt.Y * MapData.TileSize;

		#region Rotation
		if (pt.X == 0 && pt.Y == 0)
		{
			rotation.z = CornerBottomLeftRotation;
			type = EType.Corner;
		}
		else if (pt.X == 0 && pt.Y == MapData.MapHeight)
		{
			rotation.z = CornerTopLeftRotation;
			type = EType.Corner;
		}
		else if (pt.X == MapData.MapWidth && pt.Y == 0)
		{
			rotation.z = CornerBottomRightRotation;
			type = EType.Corner;
		}
		else if (pt.X == MapData.MapWidth && pt.Y == MapData.MapHeight)
		{
			rotation.z = CornerTopRightRotation;
			type = EType.Corner;
		}
		else if(pt.X == 0)
		{
			rotation.z = EdgeLeftRotation;
		}
		else if(pt.X == MapData.MapWidth)
		{
			rotation.z = EdgeRightRotation;
		}
		else if(pt.Y == 0)
		{
			rotation.z = EdgeBottomRotation;
		}
		else if(pt.Y == MapData.MapHeight)
		{
			rotation.z = EdgeTopRotation;
		}
		#endregion

		transform.position = position;
		transform.rotation = rotation;
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

		Sprite sprite = farmLandTileAtlas.GetSprite(spriteName);
		GetComponent<SpriteRenderer>().sprite = sprite;
	}
}
