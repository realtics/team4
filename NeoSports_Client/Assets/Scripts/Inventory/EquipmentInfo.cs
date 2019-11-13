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

	EType	_itemType;
	string	_itemName;
	Status	_itemStat;
	Sprite	_iconSprite;

	#region Property
	public string Name {
		get { return _itemName; }
	}

	public Status Stat {
		get { return _itemStat; }
	}

	public EType Type {
		get { return _itemType; }
	}

	public Sprite IconSprite {
		get { return _iconSprite; }
	}
	#endregion

	public EquipmentInfo(JsonData data)
	{
		_itemType = data.itemType;
		_itemName = data.itemName;
		_itemStat = data.itemStatus;
		_iconSprite = Singleton<ResourceManager>.Instance.GetUISprite(data.iconName);
	}

}
