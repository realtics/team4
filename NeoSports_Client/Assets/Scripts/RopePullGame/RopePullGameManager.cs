using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace RopePullGame
{
	public class RopePullGameManager : Singleton<RopePullGameManager>
	{
		public enum ESceneState
		{
			Prepare,
			Start,
			Play,
			SetWinner,
			WaitRestart,
		};

		// Const Variable
		const float StartWaitGameTime = 4.0f;
		const string LeftPlayerObjectName = "LeftPlayer";
		const string RightPlayerObjectName = "RightPlayer";

		// Prefab Character
		public GameObject ppiYakCharacter;
		public GameObject turkeyJellyCharacter;
		public GameObject playerPrefab;
		public GameObject AIPlayerPrefab;

		// Public Variable
		public GameObject rootCanvas;
		public GameObject playerableObjects;
		public Text leftText;
		public Text rightText;
		public Button restartButton;

		// Private Variable
		Player _player;
		Player _AIPlayer;
		ESceneState _sceneState;
		float _playTime;

		Transform _leftPlayer;
		Transform _rightPlayer;

		RopePullRope _ropePullMove;

		public ESceneState SceneState {
			get { return _sceneState; }
		}

		void Start()
		{
			_sceneState = ESceneState.Prepare;

			CachingValue();
			CreateCharacters();
			_playTime = 0.0f;
		}

		void Update()
		{
			UpdateScene();
		}

		void CachingValue()
		{
			_ropePullMove = playerableObjects.GetComponent<RopePullRope>();

			_leftPlayer = playerableObjects.transform.Find(LeftPlayerObjectName);
			_rightPlayer = playerableObjects.transform.Find(RightPlayerObjectName);
			
		}

		void CreateCharacters()
		{
			if (!IsSingleGame())
			{
				CreateMultiCharacters();
			}
			else
			{
				CreateSingleCharacter();
			}
		}
		void UpdateScene()
		{
			switch (_sceneState)
			{
				case ESceneState.Prepare:
					// DEBUG - Single Only!!

					_sceneState = ESceneState.Start;
					CommonUIManager.Instance.CreateStartGameTimer(rootCanvas, StartWaitGameTime, StartPlayGame);
					break;
				case ESceneState.Start:
					CommonUIManager.Instance.UpdateStartGameTimer();
					break;
				case ESceneState.Play:
					_playTime += Time.deltaTime;
					CommonUIManager.Instance.UpdateElapseGameTimer();
					UpdateFever();
					break;
				case ESceneState.SetWinner:
					restartButton.gameObject.SetActive(true);
					_sceneState = ESceneState.WaitRestart;
					break;
				case ESceneState.WaitRestart:
					break;
				default:
					break;
			}
		}

		void UpdateFever()
		{
			if (_playTime >= 10.0f)
			{
				_ropePullMove.SetFeverTime();
			}
		}

		void StartPlayGame()
		{
			_sceneState = ESceneState.Play;

			SetObjectsMove(true);

			CommonUIManager.Instance.CreateElapseGameTimer(rootCanvas);
		}

		public void RestartPlayGame()
		{
			_sceneState = ESceneState.Start;

			_playTime = 0.0f;
			_ropePullMove.ResetFeverTime();
			playerableObjects.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

			CommonUIManager.Instance.DestroyWinnerNotice();
			CommonUIManager.Instance.DestroyLooserNotice();
			CommonUIManager.Instance.DestroyElapseGameTimer();
			CommonUIManager.Instance.CreateStartGameTimer(rootCanvas, StartWaitGameTime, StartPlayGame);
		}

		#region Character Aniamtion & Effect
		void SetObjectsMove(bool isMove)
		{
			_ropePullMove.IsStart = isMove;
		}
		#endregion

		#region Notify Result
		public void NotifyWinner(Transform winner, string triggerSide = " ")
		{
			SetWinnerGame();
			if (!IsSingleGame())
			{
				if (winner.gameObject.CompareTag("LeftPlayer"))
				{
					if (NetworkManager.Instance.isOwnHost)
					{
						NetworkManager.Instance.SendRequestExitRoom(GAME_INDEX.ROPE_PULL, true);
						CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
					}
					else
					{
						CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
					}
				}
				else
				{
					if (!NetworkManager.Instance.isOwnHost)
					{
						NetworkManager.Instance.SendRequestExitRoom(GAME_INDEX.ROPE_PULL, true);
						CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
					}
					else
					{
						CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
					}
				}
			}
			else
			{
				if (triggerSide.CompareTo("Left") == 0)
				{
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
				}
				else
				{
					CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
				}
			}
		}

		public void NotifyLoser(Transform loser)
		{
			SetWinnerGame();
			CommonUIManager.Instance.CreateLooserNotice(rootCanvas, "패 캐릭터 이름");
		}

		void SetWinnerGame()
		{
			_sceneState = ESceneState.SetWinner;
			SetObjectsMove(false);
		}
		#endregion

		GameObject SelectInstantCharacter(CHAR_INDEX charID, Transform parent)
		{
			switch (charID)
			{
				case (CHAR_INDEX)CharacterInfo.EType.PpiYaGi:
					{
						return Instantiate(ppiYakCharacter, parent);
					}
				case (CHAR_INDEX)CharacterInfo.EType.TurkeyJelly:
					{
						return Instantiate(turkeyJellyCharacter, parent);
						
					}
				default:
					{
						break;
					}
			}
			return null;

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

		public void CreateMultiCharacters()
		{
			ppiYakCharacter.GetComponent<SpriteRenderer>().sortingOrder = 15;
			turkeyJellyCharacter.GetComponent<SpriteRenderer>().sortingOrder = 15;
			ppiYakCharacter.SetActive(true);
			turkeyJellyCharacter.SetActive(true);

			CHAR_INDEX superCharIndex = PacketQueue.Instance.superCharIndex;
			CHAR_INDEX CharIndex = PacketQueue.Instance.charIndex;

			SelectInstantCharacter(superCharIndex, _leftPlayer);
			SelectInstantCharacter(CharIndex, _rightPlayer);

			leftText.text = PacketQueue.Instance.superName;
			rightText.text = PacketQueue.Instance.guestName;

		}

		void CreateSingleCharacter()
		{
			var playerInst = Instantiate(playerPrefab, playerableObjects.transform);
			_player = playerInst.GetComponent<Player>();
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);
			_player.Initialize();
			_player.SetPlayerDirection(Player.eLookDirection.Left);

			var AIPlayerInst = Instantiate(AIPlayerPrefab, playerableObjects.transform);
			_AIPlayer = AIPlayerInst.GetComponent<Player>();
			_AIPlayer.Initialize();
			_AIPlayer.SetPlayerDirection(Player.eLookDirection.Right);

		}
		bool IsSingleGame()
		{
			if (NetworkManager.Instance != null)
			{
				return NetworkManager.Instance.IsSinglePlay();
			}
			return false;
		}
	}
}
