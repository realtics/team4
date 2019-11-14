using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InventoryManager : Singleton<PopupManager>
{
	const string CharacterDataName = "CharacterData";
	const string EquipmentDataName = "EquipmentData";

	public GameObject prefCharacterPpiYaGi;
	public GameObject prefCharacterTurkeyJelly;

	#region Property
	public Dictionary<CharacterInfo.EType, CharacterInfo> CharacterInfos { get; set; }
	public Dictionary<EquipmentInfo.EType, EquipmentInfo> EquipmentInfos { get; set; }
	public CharacterInfo CurrentCharacter { get; set; }
	public EquipmentInfo CurrentEquipment { get; set; }
	#endregion

	private void Awake()
	{
		DontDestroyOnLoad(this);

		CharacterInfos = new Dictionary<CharacterInfo.EType, CharacterInfo>();
		EquipmentInfos = new Dictionary<EquipmentInfo.EType, EquipmentInfo>();

		ReadCharacterInfos();
		ReadEquipmentInfos();

		CurrentCharacter = CharacterInfos[CharacterInfo.EType.PpiYaGi];
		CurrentEquipment = EquipmentInfos[EquipmentInfo.EType.BlackFedora];
		Singleton<GallaryManager>.Instance.SetPreview();
		//CreateJson();
	}

	#region Read Infos From Json
	void ReadCharacterInfos()
	{
		CharacterInfo.JsonData[] datas;

		string dataPath = ResourceManager.JsonDataPath + CharacterDataName;
		string data = ResourceManager.Instance.ReadJsonDataString(dataPath);

		datas = JsonConvert.DeserializeObject<CharacterInfo.JsonData[]>(data);

		foreach(var child in datas)
		{
			CharacterInfo info = new CharacterInfo(child);
			CharacterInfos.Add(child.charType, info);
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
			EquipmentInfos.Add(child.itemType, info);
		}
	}
	#endregion

	void CreateJson()
	{
		CharacterInfo[] mCharacterInfos = new CharacterInfo[2];
		CharacterInfo.JsonData[] mCharacterDatas = new CharacterInfo.JsonData[2];
		mCharacterDatas[0].charType = CharacterInfo.EType.PpiYaGi;
		mCharacterDatas[0].charName = "삐약이";
		mCharacterDatas[0].charStat = new Status(1, 1, 1, 1);
		mCharacterDatas[0].iconName = "Character_PpiYaGi";
		mCharacterInfos[0] = new CharacterInfo(mCharacterDatas[0]);

		mCharacterDatas[1].charType = CharacterInfo.EType.Unknown;
		mCharacterDatas[1].charName = "잠김";
		mCharacterDatas[1].charStat = new Status(0, 0, 0, 0);
		mCharacterDatas[1].iconName = "Icon_Group_78";
		mCharacterInfos[1] = new CharacterInfo(mCharacterDatas[1]);

		EquipmentInfo[] mEquipmentInfos = new EquipmentInfo[2];
		EquipmentInfo.JsonData[] mEquipmentDatas = new EquipmentInfo.JsonData[2];
		mEquipmentDatas[0].itemType = EquipmentInfo.EType.BlackFedora;
		mEquipmentDatas[0].itemName = "흑색 중절모";
		mEquipmentDatas[0].itemStatus = new Status(0, 1, 1, 0);
		mEquipmentDatas[0].iconName = "Equipment_Fedora_0";
		mEquipmentInfos[0] = new EquipmentInfo(mEquipmentDatas[0]);
		mEquipmentDatas[1].itemType = EquipmentInfo.EType.BrownFedora;
		mEquipmentDatas[1].itemName = "갈색 중절모";
		mEquipmentDatas[1].itemStatus = new Status(1, 0, 0, 1);
		mEquipmentDatas[1].iconName = "Equipment_Fedora_1";
		mEquipmentInfos[1] = new EquipmentInfo(mEquipmentDatas[1]);

		string tmpStr = JsonConvert.SerializeObject(mCharacterDatas);
		Debug.Log(tmpStr);

		tmpStr = JsonConvert.SerializeObject(mEquipmentDatas);
		Debug.Log(tmpStr);

		CharacterInfos.Add(mCharacterInfos[0].Type, mCharacterInfos[0]);
		CharacterInfos.Add(mCharacterInfos[1].Type, mCharacterInfos[1]);

		EquipmentInfos.Add(mEquipmentInfos[0].Type, mEquipmentInfos[0]);
		EquipmentInfos.Add(mEquipmentInfos[1].Type, mEquipmentInfos[1]);
	}

}
