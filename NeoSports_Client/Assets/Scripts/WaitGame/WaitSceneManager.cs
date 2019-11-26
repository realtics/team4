using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSceneManager : Singleton<WaitSceneManager>
{
	// Prefab Character
	public GameObject ppiYakCharacter;
	public GameObject turkeyJellyCharacter;

	void Awake()
	{
		SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type, null);
	}

	void SelectInstantCharacter(CharacterInfo.EType charType, Transform parent)
	{
		switch (charType)
		{
			case CharacterInfo.EType.PpiYaGi:
				{
					Instantiate(ppiYakCharacter, parent);
					break;
				}
			case CharacterInfo.EType.TurkeyJelly:
				{
					Instantiate(turkeyJellyCharacter, parent);
					break;
				}
			default:
				{
					break;
				}
		}

	}
}
