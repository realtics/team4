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
		const string LandDataPath = "Jsons/Farm/LandData";
		const string DecorationDataPath = "Jsons/Farm/DecorationData";
		const string ProductDataPath = "Jsons/Farm/ProductData";

		public const int MapWidth = 15;
		public const int MapHeight = 10;

		public const float TileSize = 0.48f;

		Dictionary<string, LandData> landDataDic;
		Dictionary<int, DecorationData> decorationDataDic;
		Dictionary<int, ProductData> productDataDic;

		#region Property
		public Dictionary<string, LandData> LandDataDic {
			get { return landDataDic; }
		}

		public Dictionary<int, DecorationData> DecorationDataDic {
			get { return decorationDataDic; }
		}

		public Dictionary<int, ProductData> ProductDataDic {
			get { return productDataDic; }
		}

		public Point CurrentFarmerPoint { get; set; }
		#endregion

		private void Awake()
		{
			landDataDic = new Dictionary<string, LandData>();
			decorationDataDic = new Dictionary<int, DecorationData>();
			productDataDic = new Dictionary<int, ProductData>();

			ReadLandData();
			ReadDecorationData();
			ReadProductData();
		}

		string LoadDataFromJson(string path)
		{
			TextAsset ta = Resources.Load(path) as TextAsset;
			return ta.text;
		}

		void ReadLandData()
		{
			string dataStr = LoadDataFromJson(LandDataPath);
			LandData[] dataArr = JsonConvert.DeserializeObject<LandData[]>(dataStr);

			foreach(LandData child in dataArr)
			{
				landDataDic.Add(child.type, child);
			}
			Debug.Log("Land Data Length: " + landDataDic.Count.ToString());
		}

		void ReadDecorationData()
		{
			string dataStr = LoadDataFromJson(DecorationDataPath);
			DecorationData[] dataArr = JsonConvert.DeserializeObject<DecorationData[]>(dataStr);

			foreach (DecorationData child in dataArr)
			{
				decorationDataDic.Add(child.type, child);
			}
			Debug.Log("Decoration Data Length: " + decorationDataDic.Count.ToString());
		}

		void ReadProductData()
		{
			string dataStr = LoadDataFromJson(ProductDataPath);
			ProductData[] dataArr = JsonConvert.DeserializeObject<ProductData[]>(dataStr);

			foreach (ProductData child in dataArr)
			{
				productDataDic.Add(child.type, child);
			}
			Debug.Log("Product Data Length: " + ProductDataDic.Count.ToString());
		}

	}
}
