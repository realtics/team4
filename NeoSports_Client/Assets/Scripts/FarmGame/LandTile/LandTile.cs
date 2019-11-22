using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;

public class LandTile : MonoBehaviour
{
	public enum EType
	{
		Badland,
		Grass,
		Weed,
		WeedOld,
		End
	}

	public struct TileData
	{
		public Point point;
		public EType type;
		public int typeIndex;
	}

	const int BadlandTypeCount = 2;
	const int GrassTypeCount = 2;
	const int WeedTypeCount = 3;
	const int WeedOldTypeCount = 2;

	const string BadlandSpriteName = "badland";
	const string GrassSpriteName = "grass";
	const string WeedSpriteName = "weed";
	const string WeedOldSpriteName = "weed_old";

	EType type;
	int typeIndex;

	public SpriteAtlas farmLandTileAtlas;

	#region Property
	public EType Type {
		get { return type; }
	}

	public int TypeIndex {
		get { return typeIndex; }
	}
	#endregion

	public void SetData(TileData data)
	{
		Vector3 position = Vector3.zero;
		position.x = data.point.X * MapData.TileSize;
		position.y = data.point.Y * MapData.TileSize;

		type = data.type;
		typeIndex = data.typeIndex;

		SetSprite();
	}

	public void ChangeType(EType type)
	{
		int randIndex = 0;

		switch (type)
		{
			case EType.Badland:
				randIndex = Random.Range(0, BadlandTypeCount);
				break;
			case EType.Grass:
				randIndex = Random.Range(0, GrassTypeCount);
				break;
			case EType.Weed:
				randIndex = Random.Range(0, WeedTypeCount);
				break;
			case EType.WeedOld:
				randIndex = Random.Range(0, WeedOldTypeCount);
				break;
		}

		this.type = type;
		typeIndex = randIndex;

		SetSprite();
	}

	void SetSprite()
	{
		string spriteName = string.Empty;

		switch (type)
		{
			case EType.Badland:
				spriteName += BadlandSpriteName;
				break;
			case EType.Grass:
				spriteName += GrassSpriteName;
				break;
			case EType.Weed:
				spriteName += WeedSpriteName;
				break;
			case EType.WeedOld:
				spriteName += WeedOldSpriteName;
				break;
		}

		spriteName += "_" + typeIndex.ToString();

		Sprite tileSprite = farmLandTileAtlas.GetSprite(spriteName);
		GetComponent<SpriteRenderer>().sprite = tileSprite;
	}

}
