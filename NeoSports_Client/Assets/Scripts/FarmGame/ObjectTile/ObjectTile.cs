using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace FarmGame
{
	public class ObjectTile : MonoBehaviour
	{

		public enum ETileType
		{
			Road,
			Harvest,
			Garbage,
			Decoration
		}

		protected ETileType _tileType;
		protected Point _point;

		public Point MapPoint {
			get { return _point; }
		}

	}
}
