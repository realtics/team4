using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCharacter : MonoBehaviour
{
	CharacterInfo info;

	public GameObject labelName;

	public void SetInfo(CharacterInfo info)
	{
		this.info = info;
		gameObject.GetComponent<Image>().sprite = info.IconSprite;
		labelName.GetComponent<Text>().text = info.Name;

		// On Click Event Add
		gameObject.GetComponent<Button>().onClick.AddListener(delegate () { SelectItem(); });
	}

	void SelectItem()
	{
		if(info.Type == 100)
		{
			return;
		}
		InventoryManager.Instance.CurrentCharacter = info;
		GallaryManager.Instance.SetPreview();
	}
}
