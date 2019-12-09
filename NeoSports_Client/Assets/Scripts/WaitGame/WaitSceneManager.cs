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
		_player = playerPrefab.GetComponent<Player>();

		if (InventoryManager.Instance != null)
		{
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
			nameText.text = InventoryManager.Instance.PlayerNickName;
		}
		else
		{
			SelectInstantCharacter(CharacterInfo.EType.PpiYaGi);
		}

		Instantiate(playerPrefab, null);
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
