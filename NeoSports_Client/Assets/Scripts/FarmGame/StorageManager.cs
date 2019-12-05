using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public class StorageManager : Singleton<StorageManager>
	{
		Dictionary<int, int> _cropAmount;
		Dictionary<int, int> _seedAmount;

		int _goldAmount;

		public void DeployDecoration(int type, Point pt)
		{

		}

		public void DeployProduct(int type, Point pt)
		{

		}

		public void RemoveObject(Point pt)
		{

		}

	}
}
