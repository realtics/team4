using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ObjectTileManager : Singleton<ObjectTileManager>
{
	public GameObject prefRoadTile;
	public GameObject prefProductTile;
	public GameObject objectTileGroup;

	Dictionary<Point, ObjectTile> objectTileDic;

	void Awake()
	{
		objectTileDic = new Dictionary<Point, ObjectTile>();
		InitRoadTile();
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
		RoadTile.TileData data;
		data.point = pt;
		data.material = RoadTile.EMaterial.Wire;
		data.type = RoadTile.EType.Default;
		script.SetData(data);

		objectTileDic.Add(pt, script);
	}
	#endregion


	#region Product Tile
	public void PlantProduct(Point pt, int type)
	{
		FarmUIManager.Instance.ClosePanel(FarmUIManager.ECategory.Plant);

		if (objectTileDic.ContainsKey(pt))
		{
			PopupManager.PopupData pData;
			pData.text = "한 타일에 하나의 물체만 놓을 수 있습니다.";
			pData.okFlag = true;
			pData.callBack = null;
			PopupManager.Instance.ShowPopup(pData);
			return;
		}

		GameObject tileObj = Instantiate(prefProductTile, objectTileGroup.transform);
		ProductTile script = tileObj.GetComponent<ProductTile>();
		script.PlantProduct(pt, type);

		objectTileDic.Add(pt, script);
	}

	public void LoadProduct(ProductTile.LoadData data)
	{
		GameObject tileObj = Instantiate(prefProductTile, objectTileGroup.transform);
		ProductTile script = tileObj.GetComponent<ProductTile>();
		script.LoadDataProduct(data);

		objectTileDic.Add(data.point, script);
	}
	#endregion

}
