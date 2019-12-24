#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{

	public class ObjectTileManager : Singleton<ObjectTileManager>
	{
		#region Inspector Link
		[SerializeField]
		GameObject prefRoadTile;
		[SerializeField]
		GameObject prefProductTile;
		[SerializeField]
		GameObject prefDecorationTile;
		[SerializeField]
		GameObject prefGarbageTile;
		[SerializeField]
		GameObject objectTileGroup;
		#endregion

		Dictionary<Point, ObjectTile> objectTileDic;

		private void Awake()
		{
			objectTileDic = new Dictionary<Point, ObjectTile>();
		}

		public bool HasObjectTileAtPoint(Point point)
		{
			return objectTileDic.ContainsKey(point);
		}

		public ObjectTile.ETileType GetObjectTileTypeAtPoint(Point point)
		{
			if (objectTileDic.ContainsKey(point))
			{
				return objectTileDic[point].TileType;
			}
			return ObjectTile.ETileType.None;
		}

		public void RemoveObjectTile()
		{
			Point point = MapData.Instance.CurrentFarmerPoint;

			if (objectTileDic.ContainsKey(point))
			{
				ObjectTile.ETileType tileType = objectTileDic[point].TileType;

				Destroy(objectTileDic[point].gameObject);
				objectTileDic.Remove(point);

				switch (tileType)
				{
					case ObjectTile.ETileType.Road:
						MapData.Instance.WriteSaveData(MapData.ESaveType.Road);
						break;
					case ObjectTile.ETileType.Product:
						MapData.Instance.WriteSaveData(MapData.ESaveType.Product);
						break;
					case ObjectTile.ETileType.Decoration:
						MapData.Instance.WriteSaveData(MapData.ESaveType.Decoration);
						break;
					case ObjectTile.ETileType.Garbage:
						MapData.Instance.WriteSaveData(MapData.ESaveType.Garbage);
						break;
				}
			}
		}

		#region Road Tile
		public void CreateDefaultRoadTiles(int width, int height)
		{
			Point point;

			for (int i = 0; i < width; i++)
			{
				point = new Point(i, 0);
				CreateDefaultRoadTileAtPoint(point);
				point = new Point(i, height - 1);
				CreateDefaultRoadTileAtPoint(point);
			}

			for (int i = 0; i < height; i++)
			{
				point = new Point(0, i);
				CreateDefaultRoadTileAtPoint(point);
				point = new Point(width - 1, i);
				CreateDefaultRoadTileAtPoint(point);
			}
		}

		void CreateDefaultRoadTileAtPoint(Point point)
		{
			if (objectTileDic.ContainsKey(point))
			{
				return;
			}

			GameObject obj = Instantiate(prefRoadTile, objectTileGroup.transform);
			RoadTile script = obj.GetComponent<RoadTile>();
			RoadData data;
			data.material = "wire";
			data.type = "default";
			script.DeployRoad(point, data);

			objectTileDic.Add(point, script);
		}

		public void LoadRoadTiles(RoadTile.SaveData[] datas)
		{
			if (datas == null)
			{
				return;
			}

			foreach (var item in datas)
			{
				if (objectTileDic.ContainsKey(item.point))
				{
					continue;
				}
				GameObject obj = Instantiate(prefRoadTile, objectTileGroup.transform);
				RoadTile script = obj.GetComponent<RoadTile>();
				script.SetSaveData(item);

				objectTileDic.Add(item.point, script);
			}
		}

		public RoadTile.SaveData[] GetRoadSaveDatas()
		{
			List<RoadTile.SaveData> dataList = new List<RoadTile.SaveData>();

			foreach (var item in objectTileDic)
			{
				if (item.Value.TileType == ObjectTile.ETileType.Road)
				{
					var script = item.Value as RoadTile;
					dataList.Add(script.GetSaveData());
				}
			}

			return dataList.ToArray();
		}
		#endregion


		#region Product Tile
		public void PlantProduct(Point point, int type)
		{
			FarmUIManager.Instance.ClosePanel(FarmUIManager.ECategory.Plant);

			if (CheckTileIsExist(point))
			{
				return;
			}

			GameObject obj = Instantiate(prefProductTile, objectTileGroup.transform);
			ProductTile script = obj.GetComponent<ProductTile>();
			string landType = LandTileManager.Instance.GetLandTileAtPoint(point).Type;
			script.PlantProduct(point, type, landType);

			objectTileDic.Add(point, script);
			MapData.Instance.WriteSaveData(MapData.ESaveType.Product);
		}

		public void UpdateHarvestTimeByLandChange(Point point, string landType)
		{
			if (objectTileDic.ContainsKey(point))
			{
				if(objectTileDic[point].TileType == ObjectTile.ETileType.Product)
				{
					ProductTile script = objectTileDic[point] as ProductTile;
					script.UpdateLandType(landType);
				}
			}
			MapData.Instance.WriteSaveData(MapData.ESaveType.Product);
		}

		public void LoadProductTiles(ProductTile.SaveData[] datas)
		{
			if (datas == null)
			{
				return;
			}

			foreach (var item in datas)
			{
				if (objectTileDic.ContainsKey(item.point))
				{
					continue;
				}

				GameObject obj = Instantiate(prefProductTile, objectTileGroup.transform);
				ProductTile script = obj.GetComponent<ProductTile>();
				string landType = LandTileManager.Instance.GetLandTileAtPoint(item.point).Type;
				script.SetSaveData(item, landType);

				objectTileDic.Add(item.point, script);
			}
		}

		public ProductTile.SaveData[] GetProductSaveDatas()
		{
			List<ProductTile.SaveData> dataList = new List<ProductTile.SaveData>();

			foreach (var item in objectTileDic)
			{
				if (item.Value.TileType == ObjectTile.ETileType.Product)
				{
					var script = item.Value as ProductTile;
					dataList.Add(script.GetSaveData());
				}
			}

			return dataList.ToArray();
		}
		#endregion


		#region Decoration Tile
		public void DeployDecoration(Point point, int type)
		{
			FarmUIManager.Instance.ClosePanel(FarmUIManager.ECategory.Decoration);

			if (CheckTileIsExist(point))
			{
				return;
			}

			GameObject obj = Instantiate(prefDecorationTile, objectTileGroup.transform);
			DecorationTile script = obj.GetComponent<DecorationTile>();
			script.DeployTile(point, type);

			objectTileDic.Add(point, script);
			MapData.Instance.WriteSaveData(MapData.ESaveType.Decoration);
		}

		public void LoadDecorationTiles(DecorationTile.SaveData[] datas)
		{
			if (datas == null)
			{
				return;
			}

			foreach (var item in datas)
			{
				GameObject obj = Instantiate(prefDecorationTile, objectTileGroup.transform);
				DecorationTile script = obj.GetComponent<DecorationTile>();
				script.SetSaveData(item);

				objectTileDic.Add(item.point, script);
			}
		}

		public DecorationTile.SaveData[] GetDecorationSaveDatas()
		{
			List<DecorationTile.SaveData> dataList = new List<DecorationTile.SaveData>();

			foreach (var item in objectTileDic)
			{
				if (item.Value.TileType == ObjectTile.ETileType.Decoration)
				{
					var script = item.Value as DecorationTile;
					dataList.Add(script.GetSaveData());
				}
			}

			return dataList.ToArray();
		}
		#endregion


		#region Garbage Tile
		public void FirstFarmOpenDeployGarbage()
		{
			int createCount = (MapData.MapWidth * MapData.MapHeight) / 10;

			for(int i = 0; i < createCount; i++)
			{
				CreateGarbageTile();
			}
		}

		public void CreateGarbageTile()
		{
			while (true)
			{
				Point point;
				point.X = Random.Range(0, MapData.MapWidth);
				point.Y = Random.Range(0, MapData.MapHeight);
				if (!objectTileDic.ContainsKey(point))
				{
					int typeLength = MapData.Instance.GarbageDataDic.Count;
					int randomType = Random.Range(0, typeLength);

					GameObject obj = Instantiate(prefGarbageTile, objectTileGroup.transform);
					GarbageTile script = obj.GetComponent<GarbageTile>();
					script.DeployTile(point, randomType);

					objectTileDic.Add(point, script);
					break;
				}
			}
		}

		public void LoadGarbageTile(GarbageTile.SaveData[] datas)
		{
			if (datas == null)
			{
				return;
			}

			foreach (var item in datas)
			{
				if (objectTileDic.ContainsKey(item.point))
				{
					continue;
				}

				GameObject obj = Instantiate(prefGarbageTile, objectTileGroup.transform);
				GarbageTile script = obj.GetComponent<GarbageTile>();
				script.SetSaveData(item);

				objectTileDic.Add(item.point, script);
			}
		}

		public GarbageTile.SaveData[] GetGarbageSaveDatas()
		{
			var dataList = new List<GarbageTile.SaveData>();

			foreach(var item in objectTileDic)
			{
				if(item.Value.TileType == ObjectTile.ETileType.Garbage)
				{
					var script = item.Value as GarbageTile;
					dataList.Add(script.GetSaveData());
				}
			}

			return dataList.ToArray();
		}
		#endregion


		bool CheckTileIsExist(Point point)
		{
			if (objectTileDic.ContainsKey(point))
			{
				PopupManager.PopupData pData;
				pData.text = "한 타일에 하나의 물체만 놓을 수 있습니다.";
				pData.okFlag = true;
				pData.callBack = null;
				PopupManager.Instance.ShowPopup(pData);
				return true;
			}

			return false;
		}

		public ProductTile GetProductTileAtPoint(Point point)
		{
			if (objectTileDic.ContainsKey(point))
			{
				if (objectTileDic[point].TileType == ObjectTile.ETileType.Product)
				{
					return objectTileDic[point] as ProductTile;
				}
			}
			return null;
		}

		public GarbageTile GetGarbageTileAtPoint(Point point)
		{
			if (objectTileDic.ContainsKey(point))
			{
				if (objectTileDic[point].TileType == ObjectTile.ETileType.Garbage)
				{
					return objectTileDic[point] as GarbageTile;
				}
			}
			return null;
		}
	}

}
