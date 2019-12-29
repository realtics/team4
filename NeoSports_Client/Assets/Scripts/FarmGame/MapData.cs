#pragma warning disable CS0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Compression;
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
			Garbage,
			End
		}

		const string LandDataPath = "Jsons/Farm/LandData";
		const string ProductDataPath = "Jsons/Farm/ProductData";
		const string DecorationDataPath = "Jsons/Farm/DecorationData";
		const string GarbageDataPath = "Jsons/Farm/GarbageData";

		const string LandDataBundleName = "Assets/Jsons/Farm/LandData.json";
		const string ProductDataBundleName = "Assets/Jsons/Farm/ProductData.json";
		const string DecorationDataBundleName = "Assets/Jsons/Farm/DecorationData.json";
		const string GarbageDataBundleName = "Assets/Jsons/Farm/GarbageData.json";

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
			if (FriendFarmManager.Instance == null)
			{
				CheckSaveDataIsExist();
			}
			else
			{
				LoadFriendSaveDatas();
				
			}
			CreateFarmer();
		}

		string ReadJsonDataFromResources(string dataPath)
		{
			string data;

			TextAsset ta = Resources.Load(string.Format("{0}", dataPath)) as TextAsset;
			data = ta.text;

			return data;
		}

		string ReadJsonDataFromBundle(string dataName)
		{
			string data;

			TextAsset ta = BundleManager.Instance.GetFarmBundleObject(dataName) as TextAsset;
			data = ta.text;

			return data;
		}

		#region Read Tile Data
		void ReadLandData()
		{
			string dataStr = ReadJsonDataFromBundle(LandDataBundleName);
			LandData[] dataArr = JsonConvert.DeserializeObject<LandData[]>(dataStr);

			foreach (LandData child in dataArr)
			{
				LandDataDic.Add(child.type, child);
			}
		}

		void ReadDecorationData()
		{
			string dataStr = ReadJsonDataFromBundle(DecorationDataBundleName);
			DecorationData[] dataArr = JsonConvert.DeserializeObject<DecorationData[]>(dataStr);

			foreach (DecorationData child in dataArr)
			{
				DecorationDataDic.Add(child.type, child);
			}
		}

		void ReadProductData()
		{
			string dataStr = ReadJsonDataFromBundle(ProductDataBundleName);
			ProductData[] dataArr = JsonConvert.DeserializeObject<ProductData[]>(dataStr);

			foreach (ProductData child in dataArr)
			{
				ProductDataDic.Add(child.type, child);
			}
		}

		void ReadGarbageData()
		{
			string dataStr = ReadJsonDataFromBundle(GarbageDataBundleName);
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
			}
		}

		void LoadFriendSaveDatas()
		{
			Debug.Log(FriendFarmManager.Instance.LandSaveDatas);
			LandTileManager.Instance.LoadLandTiles(FriendFarmManager.Instance.LandSaveDatas);
			ObjectTileManager.Instance.LoadRoadTiles(FriendFarmManager.Instance.RoadSaveDatas);
			ObjectTileManager.Instance.LoadDecorationTiles(FriendFarmManager.Instance.DecorationSaveDatas);
			ObjectTileManager.Instance.LoadProductTiles(FriendFarmManager.Instance.ProductSaveDatas);
			ObjectTileManager.Instance.LoadGarbageTile(FriendFarmManager.Instance.GarbageSaveDatas);
		}

		public void WriteSaveData(ESaveType type)
		{
			if (FriendFarmManager.Instance != null)
			{
				return;
			}

			switch (type)
			{
				case ESaveType.Land:
					var landDataArr = LandTileManager.Instance.GetLandSaveDatas();
					WriteTileSave(landDataArr, PrefsKey.LandSaveDataKey, type);
					break;
				case ESaveType.Road:
					var roadDataArr = ObjectTileManager.Instance.GetRoadSaveDatas();
					WriteTileSave(roadDataArr, PrefsKey.RoadSaveDataKey, type);
					break;
				case ESaveType.Product:
					var productDataArr = ObjectTileManager.Instance.GetProductSaveDatas();
					WriteTileSave(productDataArr, PrefsKey.ProductSaveDataKey, type);
					break;
				case ESaveType.Decoration:
					var decorationDataArr = ObjectTileManager.Instance.GetDecorationSaveDatas();
					WriteTileSave(decorationDataArr, PrefsKey.DecorationSaveDataKey, type);
					break;
				case ESaveType.Garbage:
					var garbageDataArr = ObjectTileManager.Instance.GetGarbageSaveDatas();
					WriteTileSave(garbageDataArr, PrefsKey.GarbageSaveDataKey, type);
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

		void WriteTileSave(object data, string key, ESaveType index)
		{
			string dataStr = JsonConvert.SerializeObject(data);
			PlayerPrefs.SetString(key, dataStr);
			string compressStr = StringCompressionHelper.Compress(dataStr);
			if(NetworkManager.Instance != null)
			{
				NetworkManager.Instance.SendFarmSaveData(index, compressStr);
			}
		}
		#endregion

		void CreateFarmer()
		{
			GameObject obj = Instantiate(prefPlayer);
			Player script = obj.GetComponent<Player>();

			if(InventoryManager.Instance != null)
			{
				script.characterPrefab = InventoryManager.Instance.CurrentCharacter.GetCharacterPrefab();
			}
			else
			{
				script.characterPrefab = prefPpiYaGi;
			}

			script.Initialize();
			script.FarmStart();

			LandTile firstTile = LandTileManager.Instance.GetLandTileAtPoint(CurrentFarmerPoint);
			script.SetTargetPosition(firstTile);
		}
	}
}
