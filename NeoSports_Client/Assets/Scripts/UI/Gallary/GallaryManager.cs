#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GallaryManager : Singleton<GallaryManager>
{

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
	[SerializeField]
	GameObject prefButtonGallaryChar;
	[SerializeField]
	GameObject prefButtonGallaryEquip;

	[SerializeField]
	GameObject panelGallary;

	// Scroll View
	[SerializeField]
	GameObject scrollViewChar;
	[SerializeField]
	GameObject scrollViewEquip;
	[SerializeField]
	GameObject scrollViewContentChar;
	[SerializeField]
	GameObject scrollViewContentEquip;

	// Preview Sprite
	[SerializeField]
	Image previewCharacterImage;
	[SerializeField]
	Image previewEquipmentImage;

	// Preview Status
	[SerializeField]
	Text strengthStatusText;
	[SerializeField]
	Text enduranceStatusText;
	[SerializeField]
	Text agilityStatusText;
	[SerializeField]
	Text luckStatusText;

	void Start()
	{
		tabDic = new Dictionary<EGallaryTab, GameObject>
		{
			{ EGallaryTab.Character, scrollViewChar },
			{ EGallaryTab.Equipment, scrollViewEquip }
		};

		EnableTab(EGallaryTab.Character);
	}

	public void EnterGallary()
	{
		currentTab = EGallaryTab.Character;

		scrollViewChar.SetActive(true);
		scrollViewEquip.SetActive(true);

		SetPreview();
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

		Status.Add(out Status result, charInfo.Stat, equipInfo.Stat);
		strengthStatusText.text = UIData.GallaryExplainStatStr + result.strength.ToString();
		enduranceStatusText.text = UIData.GallaryExplainStatEnd + result.endurance.ToString();
		agilityStatusText.text = UIData.GallaryExplainStatAgi + result.agility.ToString();
		luckStatusText.text = UIData.GallaryExplainStatLuk + result.luck.ToString();

		previewCharacterImage.sprite = charInfo.IconSprite;
		previewEquipmentImage.sprite = equipInfo.IconSprite;
	}

	#region Scroll View Func
	void MakeScrollViewCharItem()
	{
		foreach (var item in Singleton<InventoryManager>.Instance.CharacterInfos)
		{
			GameObject obj;
			obj = Instantiate(prefButtonGallaryChar, scrollViewContentChar.transform);
			obj.GetComponent<ItemCharacter>().SetInfo(item.Value);
		}
	}

	void MakeScrollViewEquipItem()
	{
		foreach (var item in Singleton<InventoryManager>.Instance.EquipmentInfos)
		{
			GameObject obj;
			obj = Instantiate(prefButtonGallaryEquip, scrollViewContentEquip.transform);
			obj.GetComponent<ItemEquipment>().SetInfo(item.Value);
		}
	}

	void ClearScrollViewItem()
	{
		foreach(Transform child in scrollViewContentChar.transform)
		{
			Destroy(child.gameObject);
		}

		foreach (Transform child in scrollViewContentEquip.transform)
		{
			Destroy(child.gameObject);
		}
	}
	#endregion

}
