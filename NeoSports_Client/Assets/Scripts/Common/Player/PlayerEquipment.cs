using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
	const string HeadSocketName = "HeadSocket";

	Player _ownPlayer;
	Character _ownCharacter;
	Transform equipTransform;
	public SpriteRenderer EquipSprite { get; set; }
	public EquipmentInfo EquipmentInfo { get; set; }
	public void InitializeEquipItem(Player player)
	{
		if (InventoryManager.Instance == null)
			return;
		EquipmentInfo equipInfo;
		if (EquipmentInfo != null)
		{
			equipInfo = EquipmentInfo;
		}
		else
		{
			equipInfo = InventoryManager.Instance.CurrentEquipment;
		}
		_ownPlayer = player;

		_ownCharacter = _ownPlayer.OwnCharacter;

		equipTransform = _ownCharacter.transform.Find(HeadSocketName);

        EquipSprite = equipTransform.gameObject.AddComponent<SpriteRenderer>();
		EquipSprite.sprite = equipInfo.IconSprite;
        EquipSprite.sortingOrder = 16;
	}

	public void SetEquipFilp(bool isFlip)
	{
		if(EquipSprite != null)
		EquipSprite.flipX = isFlip;
	}


}
