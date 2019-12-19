using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitSceneManager : Singleton<WaitSceneManager>
{
	// Prefab Character
	public GameObject ppiYakCharacter;
	public GameObject turkeyJellyCharacter;
	public GameObject playerPrefab;
	//public
	public Text nameText;
	public Text rankingText;
	//private
	Player _player;

	void Start()
	{
		var playerInst = Instantiate(playerPrefab, null);
		_player = playerInst.GetComponent<Player>();

		SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
		nameText.text = InventoryManager.Instance.PlayerNickName;
		_player.Initialize();
	}

	void SelectInstantCharacter(int charType)
	{
		CharacterInfo info = InventoryManager.Instance.GetCharacterInfo(charType);
		_player.characterPrefab = info.GetCharacterPrefab();
	}

	public void AddRankingName(string name)
	{
		rankingText.text += name;
	}

}
