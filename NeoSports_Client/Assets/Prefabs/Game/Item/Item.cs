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
		GameObject gObject = gameObject;

		object oObject = (object)gObject;
		Item iObject = oObject as Item;
	}

	void OnDisable()
	{
		
	}
}
