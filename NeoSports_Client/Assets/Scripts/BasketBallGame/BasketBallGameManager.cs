using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasketBallGame
{
	public class BasketBallGameManager : Singleton<BasketBallGameManager>
	{

		public enum EGameState
		{
			PlayerWait,
			Prepare,
			Playing,
			GameOver
		}

		const string LeftPlayerObjectName = "LeftPlayer";
		const string RightPlayerObjectName = "RightPlayer";

		const float GamePrepareTime = 4.0f;
		// Prefab Character
		public GameObject ppiYakCharacter;
		public GameObject turkeyJellyCharacter;

		public GameObject rootCanvas;
		public EGameState GameState { get; set; }
		
		public GameObject _leftPlayer;
		public GameObject _rightPlayer;
		void Awake()
		{
			instance = this;
			GameState = EGameState.PlayerWait;
		}

		void Start()
		{
			StartGame();
			if (InventoryManager.instance != null)
			{
				SelectInstantCharacter((CHAR_INDEX)InventoryManager.instance.CurrentCharacter.Type, _leftPlayer.transform);
				SelectInstantCharacter((CHAR_INDEX)CharacterInfo.EType.PpiYaGi, _rightPlayer.transform);
			}
			else
			{
				SelectInstantCharacter((CHAR_INDEX)CharacterInfo.EType.PpiYaGi, _leftPlayer.transform);
				SelectInstantCharacter((CHAR_INDEX)CharacterInfo.EType.PpiYaGi, _rightPlayer.transform);
			}
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

		void SelectInstantCharacter(CHAR_INDEX charID, Transform parent)
		{
			switch (charID)
			{
				case (CHAR_INDEX)CharacterInfo.EType.PpiYaGi:
					{
						Instantiate(ppiYakCharacter, parent);
						break;
					}
				case (CHAR_INDEX)CharacterInfo.EType.TurkeyJelly:
					{
						Instantiate(turkeyJellyCharacter, parent);
						break;
					}
				default:
					{
						break;
					}
			}

		}

	}
}
