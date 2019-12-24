﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasketBallGame
{
	public class BasketGoalCounter : MonoBehaviour
	{
		public GameObject goalInBallManager;
		public Text leftText;
		public Text rightText;

		int _leftBallCount;
		int _rightBallCount;

		void Start()
		{
			_leftBallCount = 0;
			_rightBallCount = 0;
			leftText.text = _leftBallCount.ToString();
			rightText.text = _rightBallCount.ToString();
		}

		void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.transform.CompareTag("Ball"))
			{
				++_leftBallCount;
				leftText.text = _leftBallCount.ToString();
				AudioManager.Instance.PlaySound(eSoundId.Score);
			}
			else if (collision.transform.CompareTag("AIBall"))
			{
				++_rightBallCount;
				rightText.text = _rightBallCount.ToString();
				AudioManager.Instance.PlaySound(eSoundId.Score);
			}

			BasketBall basket = collision.GetComponent<BasketBall>();
			basket.SetActiveTraill(false);
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			collision.transform.SetParent(null);
			if (collision.transform.CompareTag("Ball"))
			{
				--_leftBallCount;
				leftText.text = _leftBallCount.ToString();
			}
			if (collision.transform.CompareTag("AIBall"))
			{
				--_rightBallCount;
				rightText.text = _rightBallCount.ToString();
			}
			BasketBall basket = collision.GetComponent<BasketBall>();
			basket.SetActiveTraill(true);
		}

	}
}
