using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
	const string HeadSocketName = "HeadSocket";

	Player _ownPlayer;
	SpriteRenderer _equipSprite;
	Character _ownCharacter;

	Transform equipTransform;

	public void InitializeEquipItem(Player player)
	{
		if (InventoryManager.Instance == null)
			return;
		EquipmentInfo equipInfo = InventoryManager.Instance.CurrentEquipment;
		_ownPlayer = player;

		_ownCharacter = _ownPlayer.OwnCharacter;

		equipTransform = _ownCharacter.transform.Find(HeadSocketName);

        _equipSprite = equipTransform.gameObject.AddComponent<SpriteRenderer>();
		_equipSprite.sprite = equipInfo.IconSprite;
        _equipSprite.sortingOrder = 16;
	}

	public void SetEquipFilp(bool isFlip)
	{
		_equipSprite.flipX = isFlip;
	}
}
