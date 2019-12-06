using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using FarmGame;

public class InventoryManager : Singleton<InventoryManager>
{
	const string CharacterDataName = "CharacterData";
	const string EquipmentDataName = "EquipmentData";

	public GameObject prefCharacterPpiYaGi;
	public GameObject prefCharacterTurkeyJelly;

	#region Property
	public Dictionary<CharacterInfo.EType, CharacterInfo> CharacterInfos { get; set; }
	public Dictionary<EquipmentInfo.EType, EquipmentInfo> EquipmentInfos { get; set; }
	public Dictionary<int, int> CropAmountDic { get; set; }
	public CharacterInfo CurrentCharacter { get; set; }
	public EquipmentInfo CurrentEquipment { get; set; }
	public string PlayerNickName { get; set; }
	public bool IsNickNameDecide { get; set; }
	#endregion

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(this);

		CharacterInfos = new Dictionary<CharacterInfo.EType, CharacterInfo>();
		EquipmentInfos = new Dictionary<EquipmentInfo.EType, EquipmentInfo>();
		CropAmountDic = new Dictionary<int, int>();

		PlayerNickName = "플레이어";
		IsNickNameDecide = false;
	}

	void Start()
	{
		ReadCharacterInfos();
		ReadEquipmentInfos();

		CurrentCharacter = CharacterInfos[CharacterInfo.EType.PpiYaGi];
		CurrentEquipment = EquipmentInfos[EquipmentInfo.EType.BlackFedora];
		GallaryManager.Instance.SetPreview();
	}

	

	#region Read Infos From Json
	void ReadCharacterInfos()
	{
		CharacterInfo.JsonData[] datas;

		string dataPath = ResourceManager.JsonDataPath + CharacterDataName;
		string dataStr = ResourceManager.Instance.ReadJsonDataString(dataPath);
		datas = JsonConvert.DeserializeObject<CharacterInfo.JsonData[]>(dataStr);

		foreach (var child in datas)
		{
			CharacterInfo info = new CharacterInfo(child);
			CharacterInfos.Add((CharacterInfo.EType)child.charType, info);
		}
	}

	void ReadEquipmentInfos()
	{
		EquipmentInfo.JsonData[] datas;

		string dataPath = ResourceManager.JsonDataPath + EquipmentDataName;
		string data = ResourceManager.Instance.ReadJsonDataString(dataPath);

		datas = JsonConvert.DeserializeObject<EquipmentInfo.JsonData[]>(data);

		foreach (var child in datas)
		{
			EquipmentInfo info = new EquipmentInfo(child);
			EquipmentInfos.Add((EquipmentInfo.EType)child.itemType, info);
		}
	}
	#endregion

}
