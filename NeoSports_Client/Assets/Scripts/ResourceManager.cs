using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Newtonsoft.Json;

public class ResourceManager : Singleton<ResourceManager>
{

	struct ProductResourceData
	{
		public int type;
		public int amount;
	}

	public const string JsonDataPath = "JSons/";

	public SpriteAtlas uiAtlas;
	public SpriteAtlas gameAtals;
	public SpriteAtlas farmAtlas;

	int goldResource;
	Dictionary<int, int> productResourceDic;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);

		LoadResourceSaveData();
	}

	public string ReadJsonDataString(string dataPath)
	{
		string data;

		TextAsset ta = Resources.Load(string.Format("{0}", dataPath)) as TextAsset;
		data = ta.text;

		return data;
	}

	public Sprite GetUISprite(string name)
	{
		return uiAtlas.GetSprite(name);
	}

	public Sprite GetGameSprite(string name)
	{
		return gameAtals.GetSprite(name);
	}

	public Sprite GetFarmSprite(string name)
	{
		return farmAtlas.GetSprite(name);
	}

	public int GetGoldResource()
	{
		return goldResource;
	}

	public int SetGoldResource(int value)
	{
		goldResource = value;
		PlayerPrefs.SetInt(PrefsKey.GoldResourceSaveDataKey, goldResource);

		return goldResource;
	}

	public int AddGoldResource(int value)
	{
		goldResource += value;
		PlayerPrefs.SetInt(PrefsKey.GoldResourceSaveDataKey, goldResource);

		return goldResource;
	}

	public int GetProductResource(int type)
	{
		if (productResourceDic.ContainsKey(type))
		{
			return productResourceDic[type];
		}

		return 0;
	}

	public int SetProductResource(int type, int value)
	{
		int result;

		if (productResourceDic.ContainsKey(type))
		{
			productResourceDic[type] = value;
			result = productResourceDic[type];
		}
		else
		{
			productResourceDic.Add(type, value);
			result = value;
		}

		SaveProductResourceData();
		return result;
	}

	public int AddProductResource(int type, int value)
	{
		int result;

		if (productResourceDic.ContainsKey(type))
		{
			productResourceDic[type] += value;
			result = productResourceDic[type];
		}
		else
		{
			productResourceDic.Add(type, value);
			result = value;
		}

		SaveProductResourceData();
		return result;
	}

	void SaveProductResourceData()
	{
		ProductResourceData[] datas = new ProductResourceData[productResourceDic.Count];

		int index = 0;
		foreach(var item in productResourceDic)
		{
			datas[index].type = item.Key;
			datas[index].amount = item.Value;

			index++;
		}
		string dataStr = JsonConvert.SerializeObject(datas);

		PlayerPrefs.SetString(PrefsKey.ProductResourceSaveDataKey, dataStr);
	}

	public void LoadResourceSaveData()
	{
		goldResource = PlayerPrefs.GetInt(PrefsKey.GoldResourceSaveDataKey, 0);

		productResourceDic = new Dictionary<int, int>();
		if (PlayerPrefs.HasKey(PrefsKey.ProductResourceSaveDataKey))
		{
			string dataStr = PlayerPrefs.GetString(PrefsKey.ProductResourceSaveDataKey);
			ProductResourceData[] datas = JsonConvert.DeserializeObject<ProductResourceData[]>(dataStr);

			foreach(var item in datas)
			{
				productResourceDic.Add(item.type, item.amount);
			}
		}

#if UNITY_EDITOR
		Debug.Log("Resource Save Data Loaded");
#endif
	}

}
