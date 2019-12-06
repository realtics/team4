using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	Character _character;
	PlayerController _playerController;

	void InitPlayer(Character character, PlayerController controller)
	{
		_character = character;
		_playerController = controller;
	}

	
}
