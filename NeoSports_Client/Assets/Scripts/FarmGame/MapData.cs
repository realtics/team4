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
			Decoration,
			Garbage
		}

		const string LandDataPath = "Jsons/Farm/LandData";
		const string ProductDataPath = "Jsons/Farm/ProductData";
		const string DecorationDataPath = "Jsons/Farm/DecorationData";
		const string GarbageDataPath = "Jsons/Farm/GarbageData";

		public const int MapWidth = 15;
		public const int MapHeight = 10;

		public const float TileSize = 0.48f;

		[SerializeField]
		GameObject prefPlayer;
		[SerializeField]
		GameObject prefPpiYaGi;
		[SerializeField]
		GameObject prefTurkeyJelly;

		#region Property
		public Dictionary<string, LandData> LandDataDic { get; private set; }
		public Dictionary<int, DecorationData> DecorationDataDic { get; private set; }
		public Dictionary<int, ProductData> ProductDataDic { get; private set; }
		public Dictionary<int, GarbageData> GarbageDataDic { get; private set; }
		public Point CurrentFarmerPoint { get; set; }
		#endregion

		private void Awake()
		{
			LandDataDic = new Dictionary<string, LandData>();
			DecorationDataDic = new Dictionary<int, DecorationData>();
			ProductDataDic = new Dictionary<int, ProductData>();
			GarbageDataDic = new Dictionary<int, GarbageData>();

			// Read Json Data
			ReadLandData();
			ReadDecorationData();
			ReadProductData();
			ReadGarbageData();

		}

		private void Start()
		{
			CheckSaveDataIsExist();
			CreateFarmer();
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

			foreach (LandData child in dataArr)
			{
				LandDataDic.Add(child.type, child);
			}
		}

		void ReadDecorationData()
		{
			string dataStr = ReadJsonDataString(DecorationDataPath);
			DecorationData[] dataArr = JsonConvert.DeserializeObject<DecorationData[]>(dataStr);

			foreach (DecorationData child in dataArr)
			{
				DecorationDataDic.Add(child.type, child);
			}
		}

		void ReadProductData()
		{
			string dataStr = ReadJsonDataString(ProductDataPath);
			ProductData[] dataArr = JsonConvert.DeserializeObject<ProductData[]>(dataStr);

			foreach (ProductData child in dataArr)
			{
				ProductDataDic.Add(child.type, child);
			}
		}

		void ReadGarbageData()
		{
			string dataStr = ReadJsonDataString(GarbageDataPath);
			GarbageData[] dataArr = JsonConvert.DeserializeObject<GarbageData[]>(dataStr);

			foreach(GarbageData child in dataArr)
			{
				GarbageDataDic.Add(child.type, child);
			}
		}
		#endregion


		#region Read & Write Save Data
		void CheckSaveDataIsExist()
		{
			if (PlayerPrefs.HasKey(PrefsKey.LandSaveDataKey))
			{
				LandTile.SaveData[] dataArr = ReadTileSave<LandTile.SaveData[]>(PrefsKey.LandSaveDataKey);
				LandTileManager.Instance.LoadLandTiles(dataArr);
			}
			else
			{
				LandTileManager.Instance.CreateDefaultLandTiles(MapWidth, MapHeight);
				WriteSaveData(ESaveType.Land);
			}

			if (PlayerPrefs.HasKey(PrefsKey.RoadSaveDataKey))
			{
				RoadTile.SaveData[] dataArr = ReadTileSave<RoadTile.SaveData[]>(PrefsKey.RoadSaveDataKey);
				ObjectTileManager.Instance.LoadRoadTiles(dataArr);
			}
			else
			{
				ObjectTileManager.Instance.CreateDefaultRoadTiles(MapWidth, MapHeight);
				WriteSaveData(ESaveType.Road);
			}

			if (PlayerPrefs.HasKey(PrefsKey.ProductSaveDataKey))
			{
				ProductTile.SaveData[] dataArr = ReadTileSave<ProductTile.SaveData[]>(PrefsKey.ProductSaveDataKey);
				ObjectTileManager.Instance.LoadProductTiles(dataArr);
			}

			if (PlayerPrefs.HasKey(PrefsKey.DecorationSaveDataKey))
			{
				DecorationTile.SaveData[] dataArr = ReadTileSave<DecorationTile.SaveData[]>(PrefsKey.DecorationSaveDataKey);
				ObjectTileManager.Instance.LoadDecorationTiles(dataArr);
			}

			if (PlayerPrefs.HasKey(PrefsKey.GarbageSaveDataKey))
			{
				GarbageTile.SaveData[] dataArr = ReadTileSave<GarbageTile.SaveData[]>(PrefsKey.GarbageSaveDataKey);
				ObjectTileManager.Instance.LoadGarbageTile(dataArr);
			}
			else
			{
				ObjectTileManager.Instance.FirstFarmOpenDeployGarbage();
				WriteSaveData(ESaveType.Garbage);
				Debug.Log("Create First Garbage Tiles!");
			}
		}

		public void WriteSaveData(ESaveType type)
		{
			switch (type)
			{
				case ESaveType.Land:
					var landDataArr = LandTileManager.Instance.GetLandSaveDatas();
					WriteTileSave(landDataArr, PrefsKey.LandSaveDataKey);
					break;
				case ESaveType.Road:
					var roadDataArr = ObjectTileManager.Instance.GetRoadSaveDatas();
					WriteTileSave(roadDataArr, PrefsKey.RoadSaveDataKey);
					break;
				case ESaveType.Product:
					var productDataArr = ObjectTileManager.Instance.GetProductSaveDatas();
					WriteTileSave(productDataArr, PrefsKey.ProductSaveDataKey);
					break;
				case ESaveType.Decoration:
					var decorationDataArr = ObjectTileManager.Instance.GetDecorationSaveDatas();
					WriteTileSave(decorationDataArr, PrefsKey.DecorationSaveDataKey);
					break;
				case ESaveType.Garbage:
					var garbageDataArr = ObjectTileManager.Instance.GetGarbageSaveDatas();
					WriteTileSave(garbageDataArr, PrefsKey.GarbageSaveDataKey);
					break;
			}
		}

		public void ButtonEvent_ClearSaveData()
		{
			PlayerPrefs.DeleteKey(PrefsKey.GoldResourceSaveDataKey);
			PlayerPrefs.DeleteKey(PrefsKey.ProductResourceSaveDataKey);

			PlayerPrefs.DeleteKey(PrefsKey.LandSaveDataKey);
			PlayerPrefs.DeleteKey(PrefsKey.RoadSaveDataKey);
			PlayerPrefs.DeleteKey(PrefsKey.ProductSaveDataKey);
			PlayerPrefs.DeleteKey(PrefsKey.DecorationSaveDataKey);
			PlayerPrefs.DeleteKey(PrefsKey.GarbageSaveDataKey);
#if UNITY_EDITOR
			Debug.Log("ClearSaveData!");
#endif
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

		void CreateFarmer()
		{
			GameObject obj = Instantiate(prefPlayer);
			Player script = obj.GetComponent<Player>();
			CharacterInfo.EType type = CharacterInfo.EType.PpiYaGi; 
			if (InventoryManager.Instance != null)
			{
				type = InventoryManager.Instance.CurrentCharacter.Type;
				Debug.Log("Selected Character Call");
			}

			switch (type)
			{
				case CharacterInfo.EType.PpiYaGi:
					script.characterPrefab = prefPpiYaGi;
					Debug.Log("Current Character is PpiYaGi");
					break;
				case CharacterInfo.EType.TurkeyJelly:
					script.characterPrefab = prefTurkeyJelly;
					Debug.Log("Current Character is TurkeyJelly");
					break;
				default:
					script.characterPrefab = prefPpiYaGi;
					Debug.LogWarning("Unknown Character Type");
					break;
			}
			script.Initialize();
			script.FarmStart();
			LandTile firstTile = LandTileManager.Instance.GetLandTileAtPoint(CurrentFarmerPoint);
			script.SetTargetPosition(firstTile);
		}
	}
}
