using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		const float EndDelayTime = 5.0f;
		const int DefaultEarnGold = 10;
		// Prefab Player
		public GameObject playerPrefab;
		public GameObject AIPlayerPrefab;

		public GameObject rootCanvas;
		public BasketGoalCounter singleGoalCounter;
		public BasketStaticBasket NetworkLeftBasket;
		public BasketStaticBasket NetworkRightBasket;

		[SerializeField]
		public Vector3 leftPlayerInitPos;
		[SerializeField]
		public Vector3 rightPlayerInitPos;

		// Private Variable
		Player _player;
		Player _AIPlayer;
		Player _otherPlayer;

		bool isSingle;

		public EGameState GameState { get; set; }

		void Awake()
		{
			instance = this;
			GameState = EGameState.PlayerWait;
			InitializeGame();
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
			DecideWinner();
			Invoke(nameof(EndNetworkGamePopup), EndDelayTime);
			//CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName, 10);
			CommonUIManager.Instance.DestroyElapseGameTimer();
		}

		void CreateSingleCharacter()
		{
			var playerInst = Instantiate(playerPrefab, null);
			_player = playerInst.GetComponent<Player>();
			if (InventoryManager.Instance != null)
			{
				SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter);
			}
			else
			{
				SelectInstantCharacter(InventoryManager.Instance.DefaultCharacterInfo);
			}
			_player.Initialize();
			_player.SetPlayerDirection(Player.eLookDirection.Left);

			var AIPlayerInst = Instantiate(AIPlayerPrefab, null);
			_AIPlayer = AIPlayerInst.GetComponent<Player>();
			_AIPlayer.Initialize();
			_AIPlayer.SetPlayerDirection(Player.eLookDirection.Right);

		}
		void SelectInstantCharacter(CharacterInfo info)
		{
			_player.characterPrefab = info.GetCharacterPrefab();

		}
		void InitializeGame()
		{
			if (NetworkManager.instance == null)
			{
				isSingle = true;
			}
			else
			{
				isSingle = NetworkManager.instance.IsSinglePlay();
			}

			if (isSingle)
			{
				CreateSingleCharacter();
			}
			else
			{
				CreateMultiCharacter();
			}
		}

		void CreateMultiCharacter()
		{
			int superCharIndex = PacketQueue.Instance.superCharIndex;
			int CharIndex = PacketQueue.Instance.charIndex;

			if (NetworkManager.Instance.isOwnHost)
			{
				var playerInst = Instantiate(playerPrefab, null);
				_player = playerInst.GetComponent<Player>();

				SelectInstantCharacter(superCharIndex, _player);
				_player.Initialize();
				_player.SetPlayerDirection(Player.eLookDirection.Left);
				_player.transform.localPosition = leftPlayerInitPos;

				var otherPlayerInst = Instantiate(playerPrefab, null);
				_otherPlayer = otherPlayerInst.GetComponent<Player>();
				SelectInstantCharacter(CharIndex, _otherPlayer);
				_otherPlayer.Initialize(false);
				_otherPlayer.SetPlayerDirection(Player.eLookDirection.Right);
				_otherPlayer.transform.localPosition = rightPlayerInitPos;
			}
			else
			{
				var playerInst = Instantiate(playerPrefab, null);
				_player = playerInst.GetComponent<Player>();
				SelectInstantCharacter(CharIndex, _player);
				_player.Initialize();
				_player.SetPlayerDirection(Player.eLookDirection.Right);
				_player.transform.localPosition = rightPlayerInitPos;

				var otherPlayerInst = Instantiate(playerPrefab, null);
				_otherPlayer = otherPlayerInst.GetComponent<Player>();
				SelectInstantCharacter(superCharIndex, _otherPlayer);
				_otherPlayer.Initialize(false);
				_otherPlayer.SetPlayerDirection(Player.eLookDirection.Left);
				_otherPlayer.transform.localPosition = leftPlayerInitPos;
			}
		}

		void SelectInstantCharacter(int characterType, Player targetPlayer)
		{
			CharacterInfo info;
			if (InventoryManager.Instance.CharacterInfos.ContainsKey(characterType))
			{
				info = InventoryManager.Instance.CharacterInfos[characterType];
			}
			else
			{
				info = InventoryManager.Instance.DefaultCharacterInfo;
			}

			targetPlayer.characterPrefab = info.GetCharacterPrefab();
		}

		public void NetworkShootOtherPlayer(float power, float angleX, float angleY)
		{
			_otherPlayer.ShootBall(power, angleX, angleY);
		}

		public void NetworkMoveOtherPlayer(float x, float y,float z)
		{
			_otherPlayer.NetworkDecideTargetPos(new Vector2(x, y));
		}

		void DecideWinner()
		{
			if (isSingle)
			{
				var winner = singleGoalCounter.DecideWinner();
				if (winner == eDirection.Left)
				{
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName, DefaultEarnGold);
				}
				else if (winner == eDirection.Right)
				{
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, "AI", 0);
				}
				else if (winner == eDirection.Both)
				{
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName, DefaultEarnGold);
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, "AI", 0);
				}
			}
			else 
			{
				int leftCount = NetworkLeftBasket.GoalInCount;
				int rightCount = NetworkRightBasket.GoalInCount;

				if (leftCount > rightCount)
				{
					if (NetworkManager.Instance.isOwnHost)
						CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName, DefaultEarnGold);
					else
						CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
				}
				else if (rightCount > leftCount)
				{
					if (NetworkManager.Instance.isOwnHost)
						CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
					else
						CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName, DefaultEarnGold);
				}
				else if (rightCount == leftCount)
				{
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName, DefaultEarnGold);
				}
			}				
		}

		void EndNetworkGamePopup()
		{
			PopupManager.PopupData data;
			data.text = "게임 종료 메인메뉴로 돌아갑니다.";
			data.okFlag = true;
			data.callBack = EndCompletedNetworkGame;
			Singleton<PopupManager>.Instance.ShowPopup(data);

			NetworkManager.Instance.SendRequestExitRoom(GAME_INDEX.ROPE_PULL, true);
		}

		void EndCompletedNetworkGame()
		{
			SceneManager.LoadScene(SceneName.MenuSceneName);
		}
	}
}
