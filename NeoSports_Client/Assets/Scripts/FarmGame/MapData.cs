using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Newtonsoft.Json;

namespace FarmGame
{
	public class MapData : Singleton<MapData>
	{
		public enum ESaveType
		{
			Land,
			Road,
			Product,
			Decoration
		}

		const string LandDataPath = "Jsons/Farm/LandData";
		const string ProductDataPath = "Jsons/Farm/ProductData";
		const string DecorationDataPath = "Jsons/Farm/DecorationData";

		const string LandSaveDataKey = "Farm_Land_Tile_Save_Data";
		const string RoadSaveDataKey = "Farm_Road_Tile_Save_Data";
		const string ProductSaveDataKey = "Farm_Product_Tile_Save_Data";
		const string DecorationSaveDataKey = "Farm_Decoration_Tile_Save_Data";

		public const int MapWidth = 15;
		public const int MapHeight = 10;

		public const float TileSize = 0.48f;

		#region Property
		public Dictionary<string, LandData> LandDataDic { get; private set; }
		public Dictionary<int, DecorationData> DecorationDataDic { get; private set; }
		public Dictionary<int, ProductData> ProductDataDic { get; private set; }
		public Point CurrentFarmerPoint { get; set; }
		#endregion

		private void Awake()
		{
			LandDataDic = new Dictionary<string, LandData>();
			DecorationDataDic = new Dictionary<int, DecorationData>();
			ProductDataDic = new Dictionary<int, ProductData>();

			ReadLandData();
			ReadDecorationData();
			ReadProductData();
		}

		private void Start()
		{
			CheckSaveDataIsExist();
		}

		string ReadJsonDataString(string dataPath)
		{
			string data;

			TextAsset ta = Resources.Load(string.Format("{0}", dataPath)) as TextAsset;
			data = ta.text;

			return data;
		}

		#region Read Tile Data
		void ReadLandData()
		{
			string dataStr = ReadJsonDataString(LandDataPath);
			LandData[] dataArr = JsonConvert.DeserializeObject<LandData[]>(dataStr);

			foreach(LandData child in dataArr)
			{
				LandDataDic.Add(child.type, child);
			}
			Debug.Log("Land Data Length: " + LandDataDic.Count.ToString());
		}

		void ReadDecorationData()
		{
			string dataStr = ReadJsonDataString(DecorationDataPath);
			DecorationData[] dataArr = JsonConvert.DeserializeObject<DecorationData[]>(dataStr);

			foreach (DecorationData child in dataArr)
			{
				DecorationDataDic.Add(child.type, child);
			}
			Debug.Log("Decoration Data Length: " + DecorationDataDic.Count.ToString());
		}

		void ReadProductData()
		{
			string dataStr = ReadJsonDataString(ProductDataPath);
			ProductData[] dataArr = JsonConvert.DeserializeObject<ProductData[]>(dataStr);

			foreach (ProductData child in dataArr)
			{
				ProductDataDic.Add(child.type, child);
			}
			Debug.Log("Product Data Length: " + ProductDataDic.Count.ToString());
		}
		#endregion


		#region Read & Write Save Data
		void CheckSaveDataIsExist()
		{
			if (PlayerPrefs.HasKey(LandSaveDataKey))
			{
				LandTile.SaveData[] dataArr = ReadTileSave<LandTile.SaveData[]>(LandSaveDataKey);
				LandTileManager.Instance.LoadLandTiles(dataArr);
				Debug.Log("Load Land Save Data");
			}
			else
			{
				LandTileManager.Instance.CreateDefaultLandTiles(MapWidth, MapHeight);
				WriteSaveData(ESaveType.Land);
			}

			if (PlayerPrefs.HasKey(RoadSaveDataKey))
			{
				RoadTile.SaveData[] dataArr = ReadTileSave<RoadTile.SaveData[]>(RoadSaveDataKey);
				ObjectTileManager.Instance.LoadRoadTiles(dataArr);
				Debug.Log("Load Road Save Data");
			}
			else
			{
				ObjectTileManager.Instance.CreateDefaultRoadTiles(MapWidth, MapHeight);
				WriteSaveData(ESaveType.Road);
			}

			if (PlayerPrefs.HasKey(ProductSaveDataKey))
			{
				ProductTile.SaveData[] dataArr = ReadTileSave<ProductTile.SaveData[]>(ProductSaveDataKey);
				ObjectTileManager.Instance.LoadProductTiles(dataArr);
				Debug.Log("Load Product Save Data");
			}

			if (PlayerPrefs.HasKey(DecorationSaveDataKey))
			{
				DecorationTile.SaveData[] dataArr = ReadTileSave<DecorationTile.SaveData[]>(DecorationSaveDataKey);
				ObjectTileManager.Instance.LoadDecorationTiles(dataArr);
				Debug.Log("Load Decoration Save Data");
			}
		}

		public void WriteSaveData(ESaveType type)
		{
			switch (type)
			{
				case ESaveType.Land:
					var  landDataArr = LandTileManager.Instance.GetLandSaveDatas();
					WriteTileSave(landDataArr, LandSaveDataKey);
					break;
				case ESaveType.Road:
					var roadDataArr = ObjectTileManager.Instance.GetRoadSaveDatas();
					WriteTileSave(roadDataArr, RoadSaveDataKey);
					break;
				case ESaveType.Product:
					var productDataArr = ObjectTileManager.Instance.GetProductSaveDatas();
					WriteTileSave(productDataArr, ProductSaveDataKey);
					break;
				case ESaveType.Decoration:
					var decorationDataArr = ObjectTileManager.Instance.GetDecorationSaveDatas();
					WriteTileSave(decorationDataArr, DecorationSaveDataKey);
					break;
			}
		}

		public void ButtonEvent_ClearSaveData()
		{
			PlayerPrefs.DeleteKey(LandSaveDataKey);
			PlayerPrefs.DeleteKey(RoadSaveDataKey);
			PlayerPrefs.DeleteKey(ProductSaveDataKey);
			PlayerPrefs.DeleteKey(DecorationSaveDataKey);
		}

		T ReadTileSave<T>(string key)
		{
			string dataStr = PlayerPrefs.GetString(key);
			T dataArr = JsonConvert.DeserializeObject<T>(dataStr);
			return dataArr;
		}

		void WriteTileSave(object data, string key)
		{
			string dataStr = JsonConvert.SerializeObject(data);
			PlayerPrefs.SetString(key, dataStr);
		}
		#endregion
	}
}
