using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasketBallGame
{
	public class BasketBallPool : MonoBehaviour
	{
		[SerializeField]
		//BasketBall ballPrefab;
		BasketBall ball;

		PoolFactory ballFactory;

		void Start()
		{
			//ballFactory = new PoolFactory(ballPrefab);
		}
	}
}
