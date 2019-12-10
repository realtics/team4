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
		}

		public void CreateDefaultLandTiles(int width, int height)
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
					LandData data = MapData.Instance.LandDataDic[LandTile.BadlandType];
					script.CreateLand(pt, data);
					landTileDic.Add(pt, script);
				}
			}
		}

		public void SetLandTileType(Point point, string type)
		{
			LandData data = MapData.Instance.LandDataDic[type];
			landTileDic[point].ChangeType(data);
		}

		public void LoadLandTiles(LandTile.SaveData[] datas)
		{
			foreach(var item in datas)
			{
				GameObject obj = Instantiate(prefLandTile, landTileGroup.transform);
				LandTile script = obj.GetComponent<LandTile>();
				script.SetSaveData(item);

				landTileDic.Add(item.point, script);
			}
		}

		public LandTile.SaveData[] GetLandSaveDatas()
		{
			List<LandTile.SaveData> dataList = new List<LandTile.SaveData>();

			foreach (var item in landTileDic)
			{
				var script = item.Value;
				dataList.Add(script.GetSaveData());
			}

			return dataList.ToArray();
		}

	}

}
