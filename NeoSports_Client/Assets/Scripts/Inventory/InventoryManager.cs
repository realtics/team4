﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using FarmGame;

public class InventoryManager : Singleton<InventoryManager>
{
	public const int EmptyCharType = 100;

	const string CharacterDataName = "CharacterData";
	const string EquipmentDataName = "EquipmentData";

	#region Property
	public Dictionary<int, CharacterInfo> CharacterInfos { get; set; }
	public Dictionary<EquipmentInfo.EType, EquipmentInfo> EquipmentInfos { get; set; }
	public CharacterInfo CurrentCharacter { get; set; }
	public EquipmentInfo CurrentEquipment { get; set; }
	public string PlayerNickName { get; set; }
	public bool IsNickNameDecide { get; set; }
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
		EquipmentInfos = new Dictionary<EquipmentInfo.EType, EquipmentInfo>();

		PlayerNickName = "플레이어";
		IsNickNameDecide = false;
	}

	private void Start()
	{
		ReadCharacterInfos();
		ReadEquipmentInfos();

		CurrentCharacter = CharacterInfos[101];
		CurrentEquipment = EquipmentInfos[EquipmentInfo.EType.BlackFedora];
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

		string dataPath = ResourceManager.JsonDataPath + CharacterDataName;
		string dataStr = ResourceManager.Instance.ReadJsonDataString(dataPath);
		datas = JsonConvert.DeserializeObject<CharacterInfo.JsonData[]>(dataStr);

		foreach (var child in datas)
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
			EquipmentInfos.Add((EquipmentInfo.EType)child.itemType, info);
		}
	}
	#endregion

}
