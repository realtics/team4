using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{

	public class ObjectTileManager : Singleton<ObjectTileManager>
	{
		public GameObject prefRoadTile;
		public GameObject prefProductTile;
		public GameObject prefDecorationTile;
		public GameObject objectTileGroup;

		Dictionary<Point, ObjectTile> objectTileDic;

		private void Awake()
		{
			objectTileDic = new Dictionary<Point, ObjectTile>();
		}

		private void Start()
		{
			InitRoadTile();
		}

		public bool HasObjectTileAtPoint(Point point)
		{
			return objectTileDic.ContainsKey(point);
		}

		public void RemoveObjectTile()
		{
			Point point = MapData.Instance.CurrentFarmerPoint;

			if (objectTileDic.ContainsKey(point))
			{
				Destroy(objectTileDic[point].gameObject);
				objectTileDic.Remove(point);
			}
		}

		#region Road Tile
		void InitRoadTile()
		{
			Point pt;

			for (int i = 0; i < MapData.MapWidth; i++)
			{
				pt = new Point(i, 0);
				CreateRoadTileAtPoint(pt);
				pt = new Point(i, MapData.MapHeight - 1);
				CreateRoadTileAtPoint(pt);
			}

			for (int i = 0; i < MapData.MapHeight; i++)
			{
				pt = new Point(0, i);
				CreateRoadTileAtPoint(pt);
				pt = new Point(MapData.MapWidth - 1, i);
				CreateRoadTileAtPoint(pt);
			}
		}

		void CreateRoadTileAtPoint(Point pt)
		{
			if (objectTileDic.ContainsKey(pt))
			{
				return;
			}

			GameObject obj = Instantiate(prefRoadTile, objectTileGroup.transform);
			RoadTile script = obj.GetComponent<RoadTile>();
			RoadData data;
			data.point = pt;
			data.material = "wire";
			data.type = "default";
			script.SetData(data);

			objectTileDic.Add(pt, script);
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

			GameObject tileObj = Instantiate(prefProductTile, objectTileGroup.transform);
			ProductTile script = tileObj.GetComponent<ProductTile>();
			script.PlantProduct(point, type);

			objectTileDic.Add(point, script);
		}

		public void LoadProduct(ProductTile.SaveData data)
		{
			GameObject tileObj = Instantiate(prefProductTile, objectTileGroup.transform);
			ProductTile script = tileObj.GetComponent<ProductTile>();
			script.LoadSaveData(data);

			objectTileDic.Add(data.point, script);
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

			GameObject tileObj = Instantiate(prefDecorationTile, objectTileGroup.transform);
			DecorationTile script = tileObj.GetComponent<DecorationTile>();
			script.DeployTile(point, type);

			objectTileDic.Add(point, script);
		}

		public void LoadDecoration(DecorationTile.SaveData data)
		{

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

	}

}
