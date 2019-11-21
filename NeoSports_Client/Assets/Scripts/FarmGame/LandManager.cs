using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LandManager : Singleton<LandManager>
{

	const int MaxLandWidth = 10;
	const int MaxLandHeight = 10;

	Dictionary<Point, Land> landDic;

	void Awake()
	{
		instance = this;

		for (int i = 0; i < MaxLandWidth; i++)
		{
			for (int j = 0; j < MaxLandHeight; j++)
			{
				Point pt = new Point(i, j);
			}
		}
	}

	void ResetLands()
	{
		foreach (KeyValuePair<Point, Land> item in landDic)
		{

		}
	}



}
