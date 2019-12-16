using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EquipmentInfo
{

	public class JsonData
	{
		public int		itemType;
		public string	itemName;
		public Status	itemStatus;
		public string	iconName;
	}

	public enum EType
	{
		BlackFedora,
		BrownFedora,
        RedCap,
        DeerHorn,
        SantaHat,
		End
	}

	readonly EType _itemType;
	readonly string _itemName;
	readonly Status _itemStat;
	readonly Sprite _iconSprite;

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
		_itemType = (EType)data.itemType;
		_itemName = data.itemName;
		_itemStat = data.itemStatus;
		_iconSprite = Singleton<ResourceManager>.Instance.GetUISprite(data.iconName);
	}

}
