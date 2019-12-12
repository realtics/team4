using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
	Player _ownPlayer;
	SpriteRenderer _equipSprite;
	public void InitializeEquipItem(Player player)
	{
		if (InventoryManager.Instance == null)
			return;
		EquipmentInfo equipInfo = InventoryManager.Instance.CurrentEquipment;
		_ownPlayer = player;

		transform.parent = player.transform;
		_equipSprite = gameObject.AddComponent<SpriteRenderer>();
		_equipSprite.sprite = equipInfo.IconSprite;
		//transform.localPosition = new Vector3(player.transform.position.y + 1);
	}
}
