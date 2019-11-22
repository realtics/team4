using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

public class ObjectTileManager : Singleton<ObjectTileManager>
{
	public GameObject prefRoadTile;
	public GameObject objectTileGroup;

	Dictionary<Point, ObjectTile> objectTileDic;

	void Awake()
	{
		CreateLoadTile();
	}

	void Start()
	{

	}

	void CreateLoadTile()
	{
		for (int i = 0; i < MapData.MapWidth; i++)
		{
			for (int j = 0; j < MapData.MapHeight; j++)
			{
				Point pt = new Point(i, j);
				CheckRoadPoint(pt);
			}
		}
	}

	void CheckRoadPoint(Point pt)
	{
		if (pt.X == 0 || pt.Y == 0 || pt.X == MapData.MapWidth - 1 || pt.Y == MapData.MapHeight - 1)
		{
			GameObject obj = Instantiate(prefRoadTile, objectTileGroup.transform);
			RoadTile script = obj.GetComponent<RoadTile>();
			RoadTile.TileData data;
			data.point = pt;
			data.material = RoadTile.EMaterial.Wire;
			data.type = RoadTile.EType.Default;
			script.SetData(data);
		}
	}

}
