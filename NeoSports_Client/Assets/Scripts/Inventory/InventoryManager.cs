using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InventoryManager : Singleton<PopupManager>
{
	const string CharacterDataName = "CharacterData";
	const string EquipmentDataName = "EquipmentData";

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
		CharacterInfo[] cInfos = new CharacterInfo[2];
		CharacterInfo.JsonData[] cDatas = new CharacterInfo.JsonData[2];
		cDatas[0].charType = CharacterInfo.EType.PpiYaGi;
		cDatas[0].charName = "삐약이";
		cDatas[0].charStat = new Status(1, 1, 1, 1);
		cDatas[0].iconName = "Character_PpiYaGi";
		cInfos[0] = new CharacterInfo(cDatas[0]);

		cDatas[1].charType = CharacterInfo.EType.Unknown;
		cDatas[1].charName = "잠김";
		cDatas[1].charStat = new Status(0, 0, 0, 0);
		cDatas[1].iconName = "Icon_Group_78";
		cInfos[1] = new CharacterInfo(cDatas[1]);

		EquipmentInfo[] eInfos = new EquipmentInfo[2];
		EquipmentInfo.JsonData[] eDatas = new EquipmentInfo.JsonData[2];
		eDatas[0].itemType = EquipmentInfo.EType.BlackFedora;
		eDatas[0].itemName = "흑색 중절모";
		eDatas[0].itemStatus = new Status(0, 1, 1, 0);
		eDatas[0].iconName = "Equipment_Fedora_0";
		eInfos[0] = new EquipmentInfo(eDatas[0]);
		eDatas[1].itemType = EquipmentInfo.EType.BrownFedora;
		eDatas[1].itemName = "갈색 중절모";
		eDatas[1].itemStatus = new Status(1, 0, 0, 1);
		eDatas[1].iconName = "Equipment_Fedora_1";
		eInfos[1] = new EquipmentInfo(eDatas[1]);

		string tmpStr = JsonConvert.SerializeObject(cDatas);
		Debug.Log(tmpStr);

		tmpStr = JsonConvert.SerializeObject(eDatas);
		Debug.Log(tmpStr);

		CharacterInfos.Add(cInfos[0].Type, cInfos[0]);
		CharacterInfos.Add(cInfos[1].Type, cInfos[1]);

		EquipmentInfos.Add(eInfos[0].Type, eInfos[0]);
		EquipmentInfos.Add(eInfos[1].Type, eInfos[1]);
	}

}
