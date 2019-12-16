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

		if (InventoryManager.Instance != null)
		{
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
			nameText.text = InventoryManager.Instance.PlayerNickName;
		}
		else
		{
			SelectInstantCharacter(CharacterInfo.EType.PpiYaGi);
		}


		_player.Initialize();
	}

	void SelectInstantCharacter(CharacterInfo.EType charType)
	{
		switch (charType)
		{
			case CharacterInfo.EType.PpiYaGi:
				{
					_player.characterPrefab = ppiYakCharacter;
					break;
				}
			case CharacterInfo.EType.TurkeyJelly:
				{
					_player.characterPrefab = turkeyJellyCharacter;
					break;
				}
			default:
				{
					break;
				}
		}

	}

	public void AddRankingName(string name)
	{
		rankingText.text += name;
	}

}
