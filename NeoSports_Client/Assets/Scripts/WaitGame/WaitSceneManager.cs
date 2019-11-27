using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSceneManager : Singleton<WaitSceneManager>
{
	// Prefab Character
	public GameObject ppiYakCharacter;
	public GameObject turkeyJellyCharacter;
	//public
	public float characterSpeed;

	GameObject _Character;
	Camera _mainCam;
	Vector2 _targetPos;

	void Awake()
	{
		_Character = null;
		_mainCam = Camera.main;
		if (InventoryManager.Instance != null)
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
		else
			SelectInstantCharacter(CharacterInfo.EType.PpiYaGi);
	}

	void SelectInstantCharacter(CharacterInfo.EType charType)
	{
		switch (charType)
		{
			case CharacterInfo.EType.PpiYaGi:
				{
					_Character = new GameObject();
					Instantiate(ppiYakCharacter, _Character.transform);
					break;
				}
			case CharacterInfo.EType.TurkeyJelly:
				{
					_Character = new GameObject();
					_Character = Instantiate(turkeyJellyCharacter, _Character.transform);
					break;
				}
			default:
				{
					break;
				}
		}

	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0) == true) // mobile로 변경시 touch 이벤트 추가.
		{
			DecideTargetPos();
		}
		if ((Vector2)_Character.transform.position != _targetPos)
		{
			_Character.transform.position = Vector2.MoveTowards(_Character.transform.position, _targetPos, characterSpeed * Time.deltaTime);
		}
	}

	void DecideTargetPos()
	{
		_targetPos =  (Vector2)_mainCam.ScreenToWorldPoint(Input.mousePosition);
	}

}
