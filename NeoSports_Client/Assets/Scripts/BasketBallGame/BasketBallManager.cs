using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasketBallGame
{
	public class BasketBallManager : Singleton<BasketBallManager>
	{

		public enum EGameState
		{
			PlayerWait,
			Prepare,
			Playing,
			GameOver
		}

		const float GamePrepareTime = 4.0f;

		public GameObject rootCanvas;

		public EGameState GameState { get; set; }

		void Awake()
		{
			instance = this;
			GameState = EGameState.PlayerWait;
		}

		void Start()
		{
			StartGame();
		}

		void Update()
		{
			switch (GameState)
			{
				case EGameState.PlayerWait:
					break;
				case EGameState.Prepare:
					CommonUIManager.Instance.UpdateStartGameTimer();
					break;
				case EGameState.Playing:
					CommonUIManager.Instance.UpdateElapseGameTimer();
					break;
				case EGameState.GameOver:
					break;
				default:
					break;
			}
		}

		void StartGame()
		{
			GameState = EGameState.Prepare;
			CommonUIManager.Instance.CreateStartGameTimer(rootCanvas, GamePrepareTime, StartPlaying);
		}

		void StartPlaying()
		{
			GameState = EGameState.Playing;
			CommonUIManager.Instance.CreateElapseGameTimer(rootCanvas);
		}

		public void GameOver(string winner)
		{
			CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, "플레이어 이름");
		}

	}
}
