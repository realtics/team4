using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

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
	Pond,
	Swamp,
	Rock1,
	Rock2,
	Rock3
}

public class StorageManager : Singleton<StorageManager>
{
	Dictionary<EProduct, int> _cropAmount;
	Dictionary<EProduct, int> _seedAmount;

	int _goldAmount;

	public void DeployObject(EProduct type, Point pt)
	{

	}

	public void DeployObject(EDecoration type, Point pt)
	{

	}

	public void RemoveObject(Point pt)
	{

	}

}
