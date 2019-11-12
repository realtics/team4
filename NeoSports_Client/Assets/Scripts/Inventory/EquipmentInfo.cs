using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EquipmentInfo
{

	public struct JsonData
	{
		public EType	itemType;
		public string	itemName;
		public Status	itemStatus;
		public string	iconName;
	}

	public enum EType
	{
		BlackFedora,
		BrownFedora,
		End
	}

	EType	itemType;
	string	itemName;
	Status	itemStat;
	Sprite	iconSprite;

	#region Property
	public string Name {
		get { return itemName; }
	}

	public Status Stat {
		get { return itemStat; }
	}

	public EType Type {
		get { return itemType; }
	}

	public Sprite IconSprite {
		get { return iconSprite; }
	}
	#endregion

	public EquipmentInfo(JsonData data)
	{
		itemType = data.itemType;
		itemName = data.itemName;
		itemStat = data.itemStatus;
		iconSprite = Singleton<ResourceManager>.Instance.GetUISprite(data.iconName);
	}

}
