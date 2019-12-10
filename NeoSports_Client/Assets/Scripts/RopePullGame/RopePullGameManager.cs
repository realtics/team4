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

		// Public Variable
		public GameObject rootCanvas;
		public GameObject playerableObjects;
		public Text leftText;
		public Text rightText;
		public Button restartButton;

		// Private Variable
		GameObject _singlePlayer;
		Player _player;
		ESceneState _sceneState;
		float _playTime;

		Transform _leftPlayer;
		Transform _rightPlayer;

		RopePullRope _ropePullMove;
		Character[] _characters;
		Effect.RunnigEffect[] _runnigEffects;

		public ESceneState SceneState {
			get { return _sceneState; }
		}

		void Start()
		{
			_sceneState = ESceneState.Prepare;

			CachingValue();
			InitNetwork();
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
			_runnigEffects = playerableObjects.GetComponentsInChildren<Effect.RunnigEffect>();

			_leftPlayer = playerableObjects.transform.Find(LeftPlayerObjectName);
			_rightPlayer = playerableObjects.transform.Find(RightPlayerObjectName);

			foreach (Effect.RunnigEffect effect in _runnigEffects)
			{
				effect.EndEffect();
			}
			_singlePlayer = null;
			
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
			SetCharactersAnimation(isMove);
			SetRuningEffects(isMove);
		}

		void SetCharactersAnimation(bool isMove)
		{
			if (isMove)
			{
				foreach (Character ch in _characters)
				{
					if(ch !=null)
					ch.StartRun();
				}
			}
			else
			{
				foreach (Character ch in _characters)
				{
					if (ch != null)
					ch.EndRun();
				}
			}
		}

		void SetRuningEffects(bool isMove)
		{
			if (isMove)
			{
				foreach (Effect.RunnigEffect effect in _runnigEffects)
				{
					effect.StartEffect();
				}
			}
			else
			{
				foreach (Effect.RunnigEffect effect in _runnigEffects)
				{
					effect.EndEffect();
				}
			}
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

		void InitNetwork()
		{

		}

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

			_characters = playerableObjects.GetComponentsInChildren<Character>();
		}

		void CreateSingleCharacter()
		{
			_singlePlayer =SelectInstantCharacter((CHAR_INDEX)InventoryManager.Instance.CurrentCharacter.Type, _leftPlayer);
			_characters = playerableObjects.GetComponentsInChildren<Character>();

			var playerInst = Instantiate(playerPrefab, playerableObjects.transform);
			_player = playerPrefab.GetComponent<Player>();
			//_player.

			SelectInstantCharacter(InventoryManager.Instance.CurrentCharacter.Type);

			var shader = _singlePlayer.GetComponentInChildren<SpirteOutlineshader>();

			var playerInput = _leftPlayer.gameObject.GetComponent<RopePullInputPlayerPower>();
			playerInput.OutlineEffect += shader.PlayLineEffect;

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
