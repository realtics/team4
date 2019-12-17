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

		public float endGameTime;

		const float GamePrepareTime = 4.0f;
		// Prefab Character
		public GameObject ppiYakCharacter;
		public GameObject turkeyJellyCharacter;
		public GameObject playerPrefab;
		public GameObject AIPlayerPrefab;

		public GameObject rootCanvas;

		// Private Variable
		Player _player;
		Player _AIPlayer;

		public EGameState GameState { get; set; }
		
		void Awake()
		{
			instance = this;
			GameState = EGameState.PlayerWait;
			CreateSingleCharacter();
		}

		void Start()
		{
			StartGame();
			if (InventoryManager.instance != null)
			{

			}
			else
			{

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
					//CommonUIManager.Instance.UpdateElapseGameTimer();
					CommonUIManager.Instance.UpdateTimeoutGameTimer();
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
			//CommonUIManager.Instance.CreateElapseGameTimer(rootCanvas);
			CommonUIManager.Instance.CreateTimeoutGameTimer(rootCanvas, endGameTime, EndGame);
		}

		void EndGame()
		{
			GameState = EGameState.GameOver;
			CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, "플레이어 이름");
			CommonUIManager.Instance.DestroyElapseGameTimer();
		}

		void CreateSingleCharacter()
		{
			var playerInst = Instantiate(playerPrefab, null);
			_player = playerInst.GetComponent<Player>();
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
			_player.Initialize();
			_player.SetPlayerDirection(Player.eLookDirection.Left);

			var AIPlayerInst = Instantiate(AIPlayerPrefab, null);
			_AIPlayer = AIPlayerInst.GetComponent<Player>();
			_AIPlayer.Initialize();
			_AIPlayer.SetPlayerDirection(Player.eLookDirection.Right);

		}
		void SelectInstantCharacter(CharacterInfo.EType charType)
		{
			switch (charType)
			{
				case CharacterInfo.EType.PpiYaGi:
					{
						_player.characterPrefab = ppiYakCharacter;
						break;
					}
				case CharacterInfo.EType.TurkeyJelly:
					{
						_player.characterPrefab = turkeyJellyCharacter;
						break;
					}
				default:
					{
						_player.characterPrefab = ppiYakCharacter;
						break;
					}
			}

		}
	}
}
