using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitSceneManager : Singleton<WaitSceneManager>
{
	// Prefab Character
	public GameObject ppiYakCharacter;
	public GameObject turkeyJellyCharacter;
	//public
	public float characterSpeed;
	public Text nameText;
	public Text rankingText;

	GameObject _character;
	SpriteRenderer _charRender;
	Camera _mainCam;
	Vector2 _targetPos;

	void Awake()
	{
		_character = null;
		_mainCam = Camera.main;
		if (InventoryManager.Instance != null)
		{
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
			nameText.text = InventoryManager.Instance.PlayerNickName;
		}
		else
			SelectInstantCharacter(CharacterInfo.EType.PpiYaGi);

		_charRender = _character.GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0) == true) // mobile로 변경시 touch 이벤트 추가.
		{
			DecideTargetPos();
		}
		if ((Vector2)_character.transform.position != _targetPos)
		{
			_character.transform.position = Vector2.MoveTowards(_character.transform.position, _targetPos, characterSpeed * Time.deltaTime);
		}
	}

	void SelectInstantCharacter(CharacterInfo.EType charType)
	{
		switch (charType)
		{
			case CharacterInfo.EType.PpiYaGi:
				{
					_character = new GameObject();
					Instantiate(ppiYakCharacter, _character.transform);
					break;
				}
			case CharacterInfo.EType.TurkeyJelly:
				{
					_character = new GameObject();
					Instantiate(turkeyJellyCharacter, _character.transform);
					break;
				}
			default:
				{
					break;
				}
		}

	}

	void DecideTargetPos()
	{
		_targetPos = (Vector2)_mainCam.ScreenToWorldPoint(Input.mousePosition);

		#region DecideDirection
		if (_character.transform.position.x < _targetPos.x)
			_charRender.flipX = false;
		else
			_charRender.flipX = true;
		#endregion
	}

	public void AddRankingName(string name)
	{
		rankingText.text += name;
	}

}
