using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public class LandTileManager : Singleton<LandTileManager>
	{

		public GameObject prefLandTile;
		public GameObject landTileGroup;

		Dictionary<Point, LandTile> landTileDic;

		private void Awake()
		{
			instance = this;

			landTileDic = new Dictionary<Point, LandTile>();

			CreateLandTiles(MapData.MapWidth, MapData.MapHeight);
		}

		void CreateLandTiles(int width, int height)
		{

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
					landTileDic.Add(pt, script);
				}
			}
		}

		public void SetLandTileType(Point point, LandTile.EType type)
		{
			landTileDic[point].ChangeType(type);
		}

		void ResetLands()
		{
			foreach (KeyValuePair<Point, LandTile> item in landTileDic)
			{
				item.Value.ChangeType(LandTile.EType.Badland);
			}
		}

		public LandTile GetLandTile(Point pt)
		{
			return landTileDic[pt];
		}

	}

}
