using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmGame;

public class Item : RecycleObject
{
	Collider2D _ownCollider2d;
	SpriteRenderer _spriteRenderer;
	enum eItemCartegory
	{
		Buff,
		DeBuff,
		Gold,
	}

	void Start()
	{
		_ownCollider2d = GetComponent<Collider2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}
	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}

	void OnTriggerEnter2D(Collider2D collision)
	{ 
		var player = collision.GetComponent<Player>();
		if (player != null)
		{
			player.getItem();
			gameObject.SetActive(false); //대신 오브젝트 풀 쓸수 있으면 오브젝트 풀 사용 할 것.
		}	
	}
	
}
