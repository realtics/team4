using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LandTileManager : Singleton<LandTileManager>
{

	public GameObject prefLandTile;
	public GameObject landTileGroup;

	Dictionary<Point, LandTile> landDic;
	Size landSize; 


	public Size LandSize {
		get { return landSize; }
	}

	void Awake()
	{
		instance = this;

		landDic = new Dictionary<Point, LandTile>();

		CreateLandTiles(MapData.MapWidth, MapData.MapHeight);
	}

	void CreateLandTiles(int width, int height)
	{
		landSize = new Size(width, height);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Point pt = new Point(i, j);
				GameObject tile = Instantiate(prefLandTile, landTileGroup.transform);
				tile.name = i.ToString() + "_" + j.ToString() + "_LandTile";

				Vector3 position = Vector3.zero;
				position.x = i * MapData.TileSize;
				position.y = j * MapData.TileSize;
				tile.transform.position = position;

				LandTile script = tile.GetComponent<LandTile>();
				LandTile.TileData data;
				data.point = pt;
				data.type = LandTile.EType.Badland;
				data.typeIndex = 0;
				script.SetData(data);
				landDic.Add(pt, script);
			}
		}
	}

	void ResetLands()
	{
		foreach (KeyValuePair<Point, LandTile> item in landDic)
		{
			item.Value.ChangeType(LandTile.EType.Badland);
		}
	}



}
