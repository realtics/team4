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
		const string DecorationDataPath = "Jsons/Farm/DecorationData";
		const string ProductDataPath = "Jsons/Farm/ProductData";

		public const int MapWidth = 15;
		public const int MapHeight = 10;

		public const float TileSize = 0.48f;

		Dictionary<int, DecorationData> decorationDatas;
		Dictionary<int, ProductData> productDatas;

		#region Property
		public Dictionary<int, DecorationData> DecorationDatas {
			get { return decorationDatas; }
		}

		public Dictionary<int, ProductData> ProductDatas {
			get { return productDatas; }
		}

		public Point CurrentFarmerPoint { get; set; }
		#endregion

		void Awake()
		{
			decorationDatas = new Dictionary<int, DecorationData>();
			productDatas = new Dictionary<int, ProductData>();

			ReadDecorationData();
			ReadProductData();
		}

		string LoadDataFromJson(string path)
		{
			TextAsset ta = Resources.Load(path) as TextAsset;
			return ta.text;
		}

		void ReadDecorationData()
		{
			string dataStr = LoadDataFromJson(DecorationDataPath);
			//DecorationData[] dataArr = JsonReader.Deserialize<DecorationData[]>(dataStr);
			DecorationData[] dataArr = JsonConvert.DeserializeObject<DecorationData[]>(dataStr);

			foreach (DecorationData child in dataArr)
			{
				decorationDatas.Add(child.type, child);
			}
			Debug.Log("Decoration Data Length: " + decorationDatas.Count.ToString());
		}

		void ReadProductData()
		{
			string dataStr = LoadDataFromJson(ProductDataPath);
			//ProductData[] dataArr = JsonReader.Deserialize<ProductData[]>(dataStr);
			ProductData[] dataArr = JsonConvert.DeserializeObject<ProductData[]>(dataStr);

			foreach (ProductData child in dataArr)
			{
				productDatas.Add(child.type, child);
			}
			Debug.Log("Product Data Length: " + ProductDatas.Count.ToString());
		}

	}
}
