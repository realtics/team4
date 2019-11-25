using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace RopePullGame
{
	public class RopePullGameManager : Singleton<RopePullGame.RopePullGameManager>
	{
		enum ESceneState
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

		// Public Variable
		public GameObject rootCanvas;
		public GameObject playerableObjects;
		public Text playTimeText;
		public Text startCountingTimeText;
		public Text leftText;
		public Text rightText;
		public Button restartButton;

		// Private Variable
		ESceneState _sceneState;
		float _playTime;

		Transform _leftPlayer;
		Transform _rightPlayer;

		CharacterInfo _ownCharInfo;

		RopePullMoveRopeWithKey _ropePullMove;
		Character[] _characters;
		Effect.RunnigEffect[] _runnigEffects;

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
			_ropePullMove = playerableObjects.GetComponent<RopePullMoveRopeWithKey>();
			_characters = playerableObjects.GetComponentsInChildren<Character>();
			_runnigEffects = playerableObjects.GetComponentsInChildren<Effect.RunnigEffect>();

			_leftPlayer = playerableObjects.transform.Find(LeftPlayerObjectName);
			_rightPlayer = playerableObjects.transform.Find(RightPlayerObjectName);

			foreach (Effect.RunnigEffect effect in _runnigEffects)
			{
				effect.EndEffect();
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
					ch.StartRun();
				}
			}
			else
			{
				foreach (Character ch in _characters)
				{
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
		public void NotifyWinner(Transform winner)
		{
			SetWinnerGame();
			CommonUIManager.Instance.CreateWinnerNotice(rootCanvas, "승 캐릭터 이름");
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

		void CreateCharacters()
		{
			// 방장이면 내캐릭터 왼쪽 
			// 참가자면 내캐릭터 오른쪽

			ppiYakCharacter.GetComponent<SpriteRenderer>().sortingOrder = 15;
			turkeyJellyCharacter.GetComponent<SpriteRenderer>().sortingOrder = 15;
			ppiYakCharacter.SetActive(true);
			turkeyJellyCharacter.SetActive(true);
			_ownCharInfo = InventoryManager.Instance.CurrentCharacter;
			bool ownHost = NetworkManager.Instance.isOwnHost;
			if (ownHost)
			{
				if (CharacterInfo.EType.PpiYaGi == _ownCharInfo.Type)
				{
					Instantiate(ppiYakCharacter, _leftPlayer);
					
				}
				else
				{
					Instantiate(turkeyJellyCharacter, _leftPlayer);
				}

				CHAR_INDEX charIndex = NetworkManager.Instance.otherPlayerCharacter;
				if (charIndex == CHAR_INDEX.CHICK)
					Instantiate(ppiYakCharacter, _rightPlayer);
				else if (charIndex == CHAR_INDEX.JELLY)
					Instantiate(turkeyJellyCharacter, _rightPlayer);
			}
			else 
			{
				if (CharacterInfo.EType.PpiYaGi == _ownCharInfo.Type)
				{
					Instantiate(ppiYakCharacter, _rightPlayer);
				}
				else
				{
					Instantiate(turkeyJellyCharacter, _rightPlayer);
				}

				CHAR_INDEX charIndex = NetworkManager.Instance.otherPlayerCharacter;
				if (charIndex == CHAR_INDEX.CHICK)
					Instantiate(ppiYakCharacter, _leftPlayer);
				else if (charIndex == CHAR_INDEX.JELLY)
					Instantiate(turkeyJellyCharacter, _leftPlayer);
			}
		}
	}
}
