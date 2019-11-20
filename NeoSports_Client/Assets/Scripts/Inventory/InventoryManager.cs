using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class InventoryManager : Singleton<InventoryManager>
{
    const string CharacterDataName = "CharacterData";
    const string EquipmentDataName = "EquipmentData";
    const int InputFieldContent = 2;
    const int MaxNickName = 8;

    public GameObject prefCharacterPpiYaGi;
    public GameObject prefCharacterTurkeyJelly;
	public GameObject nickNameInputGroup;
    public GameObject nickNameInputField;

    #region Property
    public Dictionary<CharacterInfo.EType, CharacterInfo> CharacterInfos { get; set; }
    public Dictionary<EquipmentInfo.EType, EquipmentInfo> EquipmentInfos { get; set; }
    public CharacterInfo CurrentCharacter { get; set; }
    public EquipmentInfo CurrentEquipment { get; set; }
    public string PlayerNickName { get; set; }
    #endregion

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        CharacterInfos = new Dictionary<CharacterInfo.EType, CharacterInfo>();
        EquipmentInfos = new Dictionary<EquipmentInfo.EType, EquipmentInfo>();

        ReadCharacterInfos();
        ReadEquipmentInfos();

        CurrentCharacter = CharacterInfos[CharacterInfo.EType.PpiYaGi];
        CurrentEquipment = EquipmentInfos[EquipmentInfo.EType.BlackFedora];
        Singleton<GallaryManager>.Instance.SetPreview();
        //CreateJson();

        PlayerNickName = "플레이어";
    }

    #region Read Infos From Json
    void ReadCharacterInfos()
    {
        CharacterInfo.JsonData[] datas;

        string dataPath = ResourceManager.JsonDataPath + CharacterDataName;
        string data = ResourceManager.Instance.ReadJsonDataString(dataPath);

        datas = JsonConvert.DeserializeObject<CharacterInfo.JsonData[]>(data);

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
            EquipmentInfos.Add(child.itemType, info);
        }
    }
    #endregion

    public void DecideNickName()
    {
        Transform mInputFieldText = nickNameInputField.transform.GetChild(InputFieldContent);
        string inputName = mInputFieldText.GetComponent<Text>().text;
        if (!IsUseableNickName(inputName))
        {
            return;
        }

        PlayerNickName = inputName;
		nickNameInputGroup.SetActive(false);
        NetworkManager.Instance.NotifyNickName(PlayerNickName);
        Debug.Log("결정된 닉네임: " + PlayerNickName);
	}

	bool IsUseableNickName(string name)
    {
        if (name.Length == 0)
        {
            PopupManager.PopupData pData;
            pData.text = "최소 1글자 이상은 입력해야 합니다!";
            pData.okFlag = true;
            pData.callBack = null;
            PopupManager.Instance.ShowPopup(pData);
            return false;
        }

        if (name.Length > MaxNickName)
        {
            PopupManager.PopupData pData;
            pData.text = "닉네임은 최대 8글자 까지 가능합니다!";
            pData.okFlag = true;
            pData.callBack = null;
            PopupManager.Instance.ShowPopup(pData);
            return false;
        }

        return true;
    }

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
