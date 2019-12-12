using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
	Player _ownPlayer;
	SpriteRenderer _equipSprite;
    Transform _headSocket;

	public void InitializeEquipItem(Player player)
	{
		if (InventoryManager.Instance == null)
			return;
		EquipmentInfo equipInfo = InventoryManager.Instance.CurrentEquipment;
		_ownPlayer = player;

        _headSocket = transform.GetChild(0);

        _equipSprite = _headSocket.gameObject.AddComponent<SpriteRenderer>();
		_equipSprite.sprite = equipInfo.IconSprite;
        _equipSprite.sortingOrder = 16;
		//transform.localPosition = new Vector3(player.transform.position.y + 1);
	}
}
