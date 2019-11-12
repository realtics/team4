using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEquipment : MonoBehaviour
{

	EquipmentInfo info;

	public GameObject spritePreview;
	public GameObject labelName;
	public GameObject labelExplain;

	public void SetInfo(EquipmentInfo info)
	{
		this.info = info;

		string explainStr = "\n";
		if (info.Stat.strength > 0)
		{
			explainStr += UIData.GallaryExplainStatStr + info.Stat.strength.ToString() + "\n";
		}
		if (info.Stat.endurance > 0)
		{
			explainStr += UIData.GallaryExplainStatEnd + info.Stat.endurance.ToString() + "\n";
		}
		if (info.Stat.agility > 0)
		{
			explainStr += UIData.GallaryExplainStatAgi + info.Stat.agility.ToString() + "\n";
		}
		if (info.Stat.luck > 0)
		{
			explainStr += UIData.GallaryExplainStatLuk + info.Stat.luck.ToString() + "\n";
		}

		spritePreview.GetComponent<Image>().sprite = info.IconSprite;
		labelName.GetComponent<Text>().text = info.Name;
		labelExplain.GetComponent<Text>().text = explainStr;

		// On Click Event Add
		gameObject.GetComponent<Button>().onClick.AddListener(delegate () { SelectItem(); });
	}

	public void SelectItem()
	{
		Singleton<InventoryManager>.Instance.CurrentEquipment = info;
		Singleton<GallaryManager>.Instance.SetPreview();
	}
}
