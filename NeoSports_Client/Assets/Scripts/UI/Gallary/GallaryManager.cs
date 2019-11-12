using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GallaryManager : Singleton<PopupManager>
{

	const string PrefButtonGallaryCharPath = "Prefabs/UI/Prefab_Button_Gallary_Char";
	const string PrefButtonGallaryEquipPath = "Prefabs/UI/Prefab_Button_Gallary_Equip";

	public enum EGallaryTab
	{
		Character,
		Equipment
	}

	enum EPrefGallaryItemChild
	{
		Name,
		Explain,
		Sprite
	}

	EGallaryTab currentTab;
	Dictionary<EGallaryTab, GameObject> tabDic;

	// Scroll View Prefab
	GameObject prefButtonGallaryChar;
	GameObject prefButtonGallaryEquip;

	public GameObject panelGallary;

	// Scroll View
	public GameObject scrollViewChar;
	public GameObject scrollViewEquip;
	public GameObject scrollViewContentChar;
	public GameObject scrollViewContentEquip;

	// Preview Sprite
	public GameObject spritePreviewChar;
	public GameObject spritePreviewEquip;

	// Preview Status
	public GameObject labelStatusStrength;
	public GameObject labelStatusEndurance;
	public GameObject labelStatusAgility;
	public GameObject labelStatusLuck;

	private void Awake()
	{
		DontDestroyOnLoad(this);

		tabDic = new Dictionary<EGallaryTab, GameObject>();

		tabDic.Add(EGallaryTab.Character, scrollViewChar);
		tabDic.Add(EGallaryTab.Equipment, scrollViewEquip);

		prefButtonGallaryChar = Resources.Load<GameObject>(PrefButtonGallaryCharPath);
		prefButtonGallaryEquip = Resources.Load<GameObject>(PrefButtonGallaryEquipPath);

		EnableTab(EGallaryTab.Character);
	}

	public void EnterGallary()
	{
		currentTab = EGallaryTab.Character;

		scrollViewChar.SetActive(true);
		scrollViewEquip.SetActive(true);

		ClearScrollViewItem();
		MakeScrollViewCharItem();
		MakeScrollViewEquipItem();

		scrollViewEquip.SetActive(false);

	}

	#region Tab Function
	public void EnableCharacterTab()
	{
		EnableTab(EGallaryTab.Character);
	}

	public void EnableEquipmentTab()
	{
		EnableTab(EGallaryTab.Equipment);
	}

	void EnableTab(EGallaryTab tab)
	{
		if (currentTab == tab)
		{
			return;
		}
		else
		{
			currentTab = tab;
		}

		foreach (var item in tabDic)
		{
			if (item.Key == tab)
			{
				item.Value.SetActive(true);
			}
			else
			{
				item.Value.SetActive(false);
			}
		}
	}
	#endregion

	public void SetPreview()
	{
		CharacterInfo charInfo = Singleton<InventoryManager>.Instance.CurrentCharacter;
		EquipmentInfo equipInfo = Singleton<InventoryManager>.Instance.CurrentEquipment;

		Status result;
		Status.Add(charInfo.Stat, equipInfo.Stat, out result);
		labelStatusStrength.GetComponent<Text>().text = UIData.GallaryExplainStatStr + result.strength.ToString();
		labelStatusEndurance.GetComponent<Text>().text = UIData.GallaryExplainStatEnd + result.endurance.ToString();
		labelStatusAgility.GetComponent<Text>().text = UIData.GallaryExplainStatAgi + result.agility.ToString();
		labelStatusLuck.GetComponent<Text>().text = UIData.GallaryExplainStatLuk + result.luck.ToString();

		spritePreviewChar.GetComponent<Image>().sprite = charInfo.IconSprite;
		spritePreviewEquip.GetComponent<Image>().sprite = equipInfo.IconSprite;
	}

	#region Scroll View Func
	void MakeScrollViewCharItem()
	{
		foreach(var item in Singleton<InventoryManager>.Instance.CharacterInfos)
		{
			GameObject obj;
			obj = Instantiate(prefButtonGallaryChar, scrollViewContentChar.transform);
			obj.GetComponent<ItemCharacter>().SetInfo(item.Value);
		}
	}

	void MakeScrollViewEquipItem()
	{
		foreach(var item in Singleton<InventoryManager>.Instance.EquipmentInfos)
		{
			GameObject obj;
			obj = Instantiate(prefButtonGallaryEquip, scrollViewContentEquip.transform);
			obj.GetComponent<ItemEquipment>().SetInfo(item.Value);
		}
	}

	void ClearScrollViewItem()
	{
		int childCount = scrollViewContentChar.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Destroy(scrollViewContentChar.transform.GetChild(i).gameObject);
		}

		childCount = scrollViewContentEquip.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Destroy(scrollViewContentEquip.transform.GetChild(0).gameObject);
		}
	}
	#endregion

}
