using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using FarmGame;

public class InventoryManager : Singleton<InventoryManager>
{
	public const int EmptyCharType = 100;
	public const int EmptyEquipType = 0;

	const string CharacterDataBundleName = "Assets/Jsons/CharacterData.txt";
	const string EquipmentDataBundleName = "Assets/Jsons/EquipmentData.txt";

	#region Property
	public Dictionary<int, CharacterInfo> CharacterInfos { get; set; }
	public Dictionary<int, EquipmentInfo> EquipmentInfos { get; set; }
	public CharacterInfo CurrentCharacter { get; set; }
	public EquipmentInfo CurrentEquipment { get; set; }
	public string PlayerNickName { get; set; }
	public CharacterInfo DefaultCharacterInfo {
		get { return CharacterInfos[1]; }
	}
	#endregion

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);

		CharacterInfos = new Dictionary<int, CharacterInfo>();
		EquipmentInfos = new Dictionary<int, EquipmentInfo>();

		PlayerNickName = "플레이어";
	}

	private void Start()
	{
		ReadCharacterInfos();
		ReadEquipmentInfos();

		CurrentCharacter = CharacterInfos[EmptyCharType + 1];
		CurrentEquipment = EquipmentInfos[EmptyEquipType + 1];
		GallaryManager.Instance.SetPreview();
	}

	public CharacterInfo GetCharacterInfo(int charType)
	{
		if (CharacterInfos.ContainsKey(charType))
		{
			return CharacterInfos[charType];
		}
		else
		{
			return DefaultCharacterInfo;
		}

	}

	#region Read Infos From Json
	void ReadCharacterInfos()
	{
		CharacterInfo.JsonData[] datas;

		TextAsset dataAsset = BundleManager.Instance.GetCharEquipBundleObject(CharacterDataBundleName) as TextAsset;
		datas = JsonConvert.DeserializeObject<CharacterInfo.JsonData[]>(dataAsset.text);

		foreach (var child in datas)
		{
			CharacterInfo info = new CharacterInfo(child);
			CharacterInfos.Add(child.charType, info);
		}
	}

	void ReadEquipmentInfos()
	{
		EquipmentInfo.JsonData[] datas;

		TextAsset dataAsset = BundleManager.Instance.GetCharEquipBundleObject(EquipmentDataBundleName) as TextAsset;
		datas = JsonConvert.DeserializeObject<EquipmentInfo.JsonData[]>(dataAsset.text);

		foreach (var child in datas)
		{
			EquipmentInfo info = new EquipmentInfo(child);
			EquipmentInfos.Add(child.itemType, info);
		}
	}
	#endregion

}
