using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BasketBallGame
{
	public enum eDirection
	{
		Left,
		Right,
		Both,
	}
	public class BasketGoalCounter : MonoBehaviour
	{
		public GameObject goalInBallManager;
		public Text leftText;
		public Text rightText;
	
		public int LeftBallCount { get; private set; }
		public int RightBallCount { get; private set; }

        public TempEffect goalInEffectPrefab;

        void Start()
		{
			LeftBallCount = 0;
			RightBallCount = 0;
			leftText.text = LeftBallCount.ToString();
			rightText.text = RightBallCount.ToString();
		}

		void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.transform.CompareTag("Ball"))
			{
				++LeftBallCount;
				leftText.text = LeftBallCount.ToString();
				AudioManager.Instance.PlaySound(eSoundId.Score);
                goalInEffectPrefab.PlayEffect(collision.transform);

            }
			else if (collision.transform.CompareTag("AIBall"))
			{
				++RightBallCount;
				rightText.text = RightBallCount.ToString();
				AudioManager.Instance.PlaySound(eSoundId.Score);
                goalInEffectPrefab.PlayEffect(collision.transform);
            }

			BasketBall basket = collision.GetComponent<BasketBall>();
			basket.SetActiveTraill(false);
		}

		void OnTriggerExit2D(Collider2D collision)
		{
			collision.transform.SetParent(null);
			if (collision.transform.CompareTag("Ball"))
			{
				--LeftBallCount;
				leftText.text = LeftBallCount.ToString();
			}
			if (collision.transform.CompareTag("AIBall"))
			{
				--RightBallCount;
				rightText.text = RightBallCount.ToString();
			}
			BasketBall basket = collision.GetComponent<BasketBall>();
			basket.SetActiveTraill(true);
		}

		public eDirection DecideWinner()
		{
			if (LeftBallCount > RightBallCount)
				return eDirection.Left;
			else if (RightBallCount > LeftBallCount)
				return eDirection.Right;
			else
				return eDirection.Both;
		}

	}
}
