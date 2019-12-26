using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		const float EndDelayTime = 5.0f;
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
		Player _otherPlayer;
		Player _AIPlayer;
		ESceneState _sceneState;
		float _playTime;
		int _gameLevel;

		Transform _leftPlayer;
		Transform _rightPlayer;

		RopePullRope _ropePullMove;

		public ESceneState SceneState {
			get { return _sceneState; }
		}

		void Start()
		{
			_sceneState = ESceneState.Prepare;
			_gameLevel = 1;
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
					//Debug. Single
					//Invoke(nameof(PopUpNextLevel),1.5f);
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
				if (triggerSide.CompareTo("Left") == 0)
				{
					if (NetworkManager.Instance.isOwnHost)
					{					
						CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName,10);
						Invoke(nameof(EndNetworkGamePopup), EndDelayTime);
						return;
					}
					else
					{
						CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
						Invoke(nameof(EndNetworkGamePopup), EndDelayTime);
						return;
					}
				}
				else
				{
					if (!NetworkManager.Instance.isOwnHost)
					{
						NetworkManager.Instance.SendRequestExitRoom(GAME_INDEX.ROPE_PULL, true);
						CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName,10);
						Invoke(nameof(EndNetworkGamePopup), EndDelayTime);
						return;
					}
					else
					{
						CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
						Invoke(nameof(EndNetworkGamePopup), EndDelayTime);
						return;
					}
				}
			}
			else
			{
				if (triggerSide.CompareTo("Left") == 0)
				{
					CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, InventoryManager.Instance.PlayerNickName,10);
					return;
				}
				else
				{
					CommonUIManager.Instance.CreateLooserNotice(rootCanvas, InventoryManager.Instance.PlayerNickName);
					return;
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

		GameObject SelectInstantCharacter(int charType, Transform parent)
		{
			CharacterInfo info;
			if (InventoryManager.Instance.CharacterInfos.ContainsKey(charType))
			{
				info = InventoryManager.Instance.CharacterInfos[charType];
			}
			else
			{
				info = InventoryManager.Instance.DefaultCharacterInfo;
			}

			GameObject prefab = info.GetCharacterPrefab();
			return Instantiate(prefab, parent);
		}

		void SelectInstantCharacter(int charType, Player targetPlayer)
		{
			CharacterInfo info = InventoryManager.Instance.GetCharacterInfo(charType);
			targetPlayer.characterPrefab = info.GetCharacterPrefab();
		}

		public void CreateMultiCharacters()
		{

			ppiYakCharacter.GetComponent<SpriteRenderer>().sortingOrder = 15;
			turkeyJellyCharacter.GetComponent<SpriteRenderer>().sortingOrder = 15;
			ppiYakCharacter.SetActive(true);
			turkeyJellyCharacter.SetActive(true);

			int superCharIndex = PacketQueue.Instance.superCharIndex;
			int CharIndex = PacketQueue.Instance.charIndex;

			//SelectInstantCharacter(superCharIndex, _leftPlayer);
			//SelectInstantCharacter(CharIndex, _rightPlayer);
			if (NetworkManager.Instance.isOwnHost)
			{
				var playerInst = Instantiate(playerPrefab, _leftPlayer.transform);
				_player = playerInst.GetComponent<Player>();
				SelectInstantCharacter(superCharIndex, _player);
				_player.Initialize();
				_player.SetPlayerDirection(Player.eLookDirection.Left);
				_player.transform.localPosition = new Vector3(0.0f, 0.0f,0.0f);

				var otherPlayerInst = Instantiate(playerPrefab, _rightPlayer.transform);
				_otherPlayer = otherPlayerInst.GetComponent<Player>();
				SelectInstantCharacter(CharIndex, _otherPlayer);
				_otherPlayer.Initialize();
				_otherPlayer.SetPlayerDirection(Player.eLookDirection.Right);
				_otherPlayer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

				leftText.text = PacketQueue.Instance.superName;
				rightText.text = PacketQueue.Instance.guestName;

			}
			else 
			{
				var playerInst = Instantiate(playerPrefab, _rightPlayer.transform);
				_player = playerInst.GetComponent<Player>();
				SelectInstantCharacter(CharIndex, _player);
				_player.Initialize();
				_player.SetPlayerDirection(Player.eLookDirection.Right);
				_player.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

				var otherPlayerInst = Instantiate(playerPrefab, _leftPlayer.transform);
				_otherPlayer = otherPlayerInst.GetComponent<Player>();
				SelectInstantCharacter(superCharIndex, _otherPlayer);
				_otherPlayer.Initialize();
				_otherPlayer.SetPlayerDirection(Player.eLookDirection.Left);
				_otherPlayer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

				leftText.text = PacketQueue.Instance.superName;
				rightText.text = PacketQueue.Instance.guestName;
			}



		}

		void CreateSingleCharacter()
		{
			var playerInst = Instantiate(playerPrefab, playerableObjects.transform);
			_player = playerInst.GetComponent<Player>();
			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type,_player);
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

		void EndNetworkGamePopup()
		{
			PopupManager.PopupData data;
			data.text = "게임 종료 메인메뉴로 돌아갑니다.";
			data.okFlag = true;
			data.callBack = EndCompletedNetworkGame;
			Singleton<PopupManager>.Instance.ShowPopup(data);

			NetworkManager.Instance.SendRequestExitRoom(GAME_INDEX.ROPE_PULL, true);
		}

		void EndSingleGamePopUp()
		{
			
		}

		void EndCompletedNetworkGame()
		{
			SceneManager.LoadScene(SceneName.MenuSceneName);
		}

		void MoveNextLevel()
		{
			Debug.Log("NextLeel");
			//DeleteSinglePlayers();

			_gameLevel += 1;
		}

		void DeleteSinglePlayers()
		{
			Destroy(_player.transform.gameObject);
			Destroy(_AIPlayer.transform.gameObject);
		}
	}
}
