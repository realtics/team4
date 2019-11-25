using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : Singleton<StorageManager>
{

	public enum EProduct
	{
		Agave,
		Aloe,
		Orange,
		Apple,
		Mango,
		Tomato,
		Clover,
		BlueFlower,
		YelloFlower,
		Lavender,
		Potato,
		Strawberry,
		SweetPotato
	}

	public enum EDecoration
	{

	}

	Dictionary<EProduct, int> _cropAmount;
	Dictionary<EProduct, int> _seedAmount;

	int _goldAmount;
}
