using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGameManager : Singleton<JumpGameManager>
{

	public GameObject leftPlayer;
	Transform leftCharacter;

	private void Start()
	{
		leftCharacter = leftPlayer.transform.GetChild(0);
	}

	private void Update()
	{
		KeyInput();
	}

	void KeyInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			leftCharacter.GetComponent<Character>().StartJump();
		}
	}

}
