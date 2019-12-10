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
			None,
			Road,
			Product,
			Garbage,
			Decoration
		}

		protected ETileType _tileType;
		protected Point _point;

		public Point MapPoint {
			get { return _point; }
		}

		public ETileType TileType {
			get { return _tileType; }
		}

	}
}
