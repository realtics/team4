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

	const string CharEquipAtlasBundleName = "Assets/Sprites/Atlas/UIAtlas.spriteatlas";
	const string FarmAtlasBundleName = "Assets/Sprites/Atlas/FarmAtlas.spriteatlas";

	SpriteAtlas uiAtlas;
	public SpriteAtlas gameAtals;
	SpriteAtlas farmAtlas;

	int goldResource;

	public int ClientId { get; private set; }
	public Dictionary<int, int> ProductResourceDic { get; private set; }

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);

		LoadAtlasFromBundle();
		LoadClientId();
		LoadResourceSaveData();
	}

	void LoadAtlasFromBundle()
	{
		uiAtlas = BundleManager.Instance.GetCharEquipBundleObject(CharEquipAtlasBundleName) as SpriteAtlas;
		farmAtlas = BundleManager.Instance.GetFarmBundleObject(FarmAtlasBundleName) as SpriteAtlas;
	}

	public string ReadJsonDataString(string dataPath)
	{
		string data;

		TextAsset ta = Resources.Load(string.Format("{0}", dataPath)) as TextAsset;
		data = ta.text;

		return data;
	}

	#region Login Info
	void LoadClientId()
	{
		ClientId = PlayerPrefs.GetInt(PrefsKey.ClientIdKey, 0);
	}

	public void SetClientId(int clientId)
	{
		ClientId = clientId;
		PlayerPrefs.SetInt(PrefsKey.ClientIdKey, ClientId);
	}
	#endregion


	#region Sprite Getter
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
	#endregion


	#region Resource Get & Set / Load
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
		if (ProductResourceDic.ContainsKey(type))
		{
			return ProductResourceDic[type];
		}

		return 0;
	}

	public int SetProductResource(int type, int value)
	{
		int result;

		if (ProductResourceDic.ContainsKey(type))
		{
			ProductResourceDic[type] = value;
			result = ProductResourceDic[type];
		}
		else
		{
			ProductResourceDic.Add(type, value);
			result = value;
		}

		SaveProductResourceData();
		return result;
	}

	public int AddProductResource(int type, int value)
	{
		int result;

		if (ProductResourceDic.ContainsKey(type))
		{
			ProductResourceDic[type] += value;
			result = ProductResourceDic[type];
		}
		else
		{
			ProductResourceDic.Add(type, value);
			result = value;
		}

		SaveProductResourceData();
		return result;
	}

	void SaveProductResourceData()
	{
		ProductResourceData[] datas = new ProductResourceData[ProductResourceDic.Count];

		int index = 0;
		foreach (var item in ProductResourceDic)
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

		ProductResourceDic = new Dictionary<int, int>();
		if (PlayerPrefs.HasKey(PrefsKey.ProductResourceSaveDataKey))
		{
			string dataStr = PlayerPrefs.GetString(PrefsKey.ProductResourceSaveDataKey);
			ProductResourceData[] datas = JsonConvert.DeserializeObject<ProductResourceData[]>(dataStr);

			foreach (var item in datas)
			{
				ProductResourceDic.Add(item.type, item.amount);
			}
		}

#if UNITY_EDITOR
		Debug.Log("Resource Save Data Loaded");
#endif
	}
	#endregion

}
