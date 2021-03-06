﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketStaticBasket : MonoBehaviour
{
	public Text _goalSignText;
    public TempEffect goalInEffectPrefab;

	[HideInInspector]
	public int GoalInCount { get; private set; }

    void Start()
    {
		GoalInCount = 0;
		_goalSignText.text = GoalInCount.ToString();

	}


	void OnTriggerEnter2D(Collider2D collision)
	{
		GoalInCount++;
        goalInEffectPrefab.PlayEffect(collision.transform);
        _goalSignText.text = GoalInCount.ToString(); // text에 변수 바인딩 할수 있는지 물어볼 것
		AudioManager.Instance.PlaySound(eSoundId.Score);
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		GoalInCount--;
		_goalSignText.text = GoalInCount.ToString();
	}
}
