using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		instance = this;
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

		datas = JsonUtility.FromJson<CharacterInfo.JsonData[]>(data);

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

		datas = JsonUtility.FromJson<EquipmentInfo.JsonData[]>(data);

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
        NetworkManager.Instance.SendNickName(PlayerNickName);
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

}
