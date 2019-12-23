using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace FarmGame
{
	public class FriendFarmManager : Singleton<FriendFarmManager>
	{

		LandTile.SaveData[] _landSaveDatas;
		RoadTile.SaveData[] _roadSaveDatas;
		ProductTile.SaveData[] _productSaveDatas;
		DecorationTile.SaveData[] _decorationSaveDatas;
		GarbageTile.SaveData[] _garbageSaveDatas;


		public LandTile.SaveData[] LandSaveDatas {
			get { return _landSaveDatas; }
		}
		public RoadTile.SaveData[] RoadSaveDatas {
			get { return _roadSaveDatas; }
		}
		public ProductTile.SaveData[] ProductSaveDatas {
			get { return _productSaveDatas; }
		}
		public DecorationTile.SaveData[] DecorationSaveDatas {
			get { return _decorationSaveDatas; }
		}
		public GarbageTile.SaveData[] GarbageSaveDatas {
			get { return _garbageSaveDatas; }
		}
		public bool IsDataLoaded { get; private set; } = false;
		public bool[] IsDataRecv { get; set; }
		public int FriendFarmClientId { get; set; } = 0;

		private void Awake()
		{
			instance = this;
			DontDestroyOnLoad(this);

			IsDataRecv = new bool[(int)MapData.ESaveType.End];
			Array.Clear(IsDataRecv, 0, IsDataRecv.Length);

			// DEBUG!
			// DebugAwake();
		}

		public void DestroySelf()
		{
			instance = null;
			Destroy(gameObject);
		}

		public void RequestFriendFarmData()
		{
			if(FriendFarmClientId == 0)
			{
				Debug.Log("Friend Farm Client Id is Not Setted");
				return;
			}

			NetworkManager.Instance.SendFriendFarmDataRequest(FriendFarmClientId);
		}

		public void LoadFarmSaveDatas(string compressionData, MapData.ESaveType saveType)
		{
			if(compressionData != string.Empty)
			{
				string originalStr = StringCompressionHelper.Decompress(compressionData);

				switch (saveType)
				{
					case MapData.ESaveType.Land:
						_landSaveDatas = DeserializeData<LandTile.SaveData>(originalStr);
						break;
					case MapData.ESaveType.Road:
						_roadSaveDatas = DeserializeData<RoadTile.SaveData>(originalStr);
						break;
					case MapData.ESaveType.Product:
						_productSaveDatas = DeserializeData<ProductTile.SaveData>(originalStr);
						break;
					case MapData.ESaveType.Decoration:
						_decorationSaveDatas = DeserializeData<DecorationTile.SaveData>(originalStr);
						break;
					case MapData.ESaveType.Garbage:
						_garbageSaveDatas = DeserializeData<GarbageTile.SaveData>(originalStr);
						break;
				}
			}

			IsDataRecv[(int)saveType] = true;

			bool allRecv = IsDataRecv[0];
			foreach(var flag in IsDataRecv)
			{
				allRecv = allRecv && flag;
			}

			IsDataLoaded = allRecv;
		}

		T[] DeserializeData<T>(string dataStr)
		{
			T[] dataArr = JsonConvert.DeserializeObject<T[]>(dataStr);
			return dataArr;
		}

		#region DEBUG
		void DebugAwake()
		{
			DebugSetData(PrefsKey.LandSaveDataKey, ref _landSaveDatas);
			DebugSetData(PrefsKey.RoadSaveDataKey, ref _roadSaveDatas);
			DebugSetData(PrefsKey.ProductSaveDataKey, ref _productSaveDatas);
			DebugSetData(PrefsKey.DecorationSaveDataKey, ref _decorationSaveDatas);
			DebugSetData(PrefsKey.GarbageSaveDataKey, ref _garbageSaveDatas);
			IsDataLoaded = true;
		}

		void DebugSetData<T>(string prefKey, ref T[] output)
		{
			if (PlayerPrefs.HasKey(prefKey))
			{
				string dataStr = PlayerPrefs.GetString(prefKey);
				output = DeserializeData<T>(dataStr);
			}
		}
		#endregion

	}
}
