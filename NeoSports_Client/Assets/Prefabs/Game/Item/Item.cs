using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmGame;

public class Item : RecycleObject
{
	Collider2D _ownCollider2d;

	void Start()
	{
		_ownCollider2d = GetComponent<Collider2D>();	
	}
	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log("아이템");
		// 아이템 효과 적용.

		// 사라지는거 오브젝트 풀.
	}
	
}
