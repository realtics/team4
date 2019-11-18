using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JumpRopeGame
{
	public class JumpGameManager : Singleton<JumpGameManager>
	{
		enum EGameState
		{
			PlayerWait,
			Prepare,
			Playing,
			GameOver
		}

		enum ETimingBarDirection
		{
			Left = -1,
			Right = 1
		}

		#region Constant Variable
		const float GameStartPrepareTime = 5.0f;
		const string GameStartPrepareTimeStartText = "GAME START!";

		const float TimingBarPointerInitialSpeed = 150.0f;
		#endregion

		#region public Variable
		// UI Object
		public GameObject timingBarBackUi;
		public GameObject timingBarSuccessZoneUi;
		public GameObject timingBarPointerUi;
		public GameObject startGameTimerUi;
		public GameObject elapseGameTimerUi;
		public GameObject winnerLabelUi;
		public GameObject jumpButtonUi;

		// Game Object
		public GameObject leftPlayer;
		#endregion

		#region Private Variable
		EGameState _gameState;
		Character _leftCharacter;

		// Timing Bar
		RectTransform _timingBarBackTransform;
		RectTransform _timingBarSuccessZoneTransform;
		RectTransform _timingBarPointerTransform;

		Text _startGameTimerText;
		Text _elapseGameTimerText;

		BoxCollider _jumpButtonCollider;

		ETimingBarDirection _timingBarPointerDirection;
		float _timingBarPointerSpeed;
		bool _isJumpSuccess;

		// Timer
		float _startGameTimer;
		float _elpaseGameTimer;
		#endregion

		private void Start()
		{
			InitComponent();

			_gameState = EGameState.PlayerWait;

			_timingBarPointerDirection = ETimingBarDirection.Right;
			_timingBarPointerSpeed = TimingBarPointerInitialSpeed;
			_isJumpSuccess = false;

			_startGameTimer = GameStartPrepareTime;
			_elpaseGameTimer = 0.0f;

			StartGame();
		}

		void InitComponent()
		{
			_leftCharacter = leftPlayer.transform.GetChild(0).GetComponent<Character>();

			_timingBarBackTransform = timingBarBackUi.GetComponent<RectTransform>();
			_timingBarSuccessZoneTransform = timingBarSuccessZoneUi.GetComponent<RectTransform>();
			_timingBarPointerTransform = timingBarPointerUi.GetComponent<RectTransform>();

			_startGameTimerText = startGameTimerUi.GetComponent<Text>();
			_elapseGameTimerText = elapseGameTimerUi.GetComponent<Text>();

			_jumpButtonCollider = jumpButtonUi.GetComponent<BoxCollider>();
		}

		private void Update()
		{
			switch (_gameState)
			{
				case EGameState.PlayerWait:
					break;
				case EGameState.Prepare:
					UpdateStartGameTimer();
					break;
				case EGameState.Playing:
					UpdateElapseGameTimer();

					CheckJumpCompleteAtSuccessZone();
					UpdateTimingBarPointerPosition();
					UpdateTimingBarPointerDirection();

					InputTouchScreen();
					break;
				case EGameState.GameOver:
					break;
				default:
					break;
			}
		}

		#region Update Timer
		void UpdateStartGameTimer()
		{
			_startGameTimer -= Time.deltaTime;

			if (_startGameTimer > 1.0f)
			{
				_startGameTimerText.text = ((int)_startGameTimer).ToString();
			}
			else if (_startGameTimer > 0.0f)
			{
				_startGameTimerText.text = GameStartPrepareTimeStartText;
			}
			else
			{
				_gameState = EGameState.Playing;

				GameObject mUIStartGameTimerParent = startGameTimerUi.transform.parent.gameObject;
				mUIStartGameTimerParent.SetActive(false);
				elapseGameTimerUi.SetActive(true);
			}
		}

		void UpdateElapseGameTimer()
		{
			_elpaseGameTimer += Time.deltaTime;

			_elapseGameTimerText.text = ((int)_elpaseGameTimer).ToString();
		}
		#endregion


		#region Jump Timing Bar Func
		void UpdateTimingBarPointerPosition()
		{
			Vector2 mPointerPosition = _timingBarPointerTransform.anchoredPosition;

			mPointerPosition.x += _timingBarPointerSpeed * Time.deltaTime * (float)_timingBarPointerDirection;
			_timingBarPointerTransform.anchoredPosition = mPointerPosition;
		}

		void UpdateTimingBarPointerDirection()
		{
			Vector2 mPointerPosition = _timingBarPointerTransform.anchoredPosition;
			Rect mBackRect = _timingBarBackTransform.rect;

			if (mPointerPosition.x < mBackRect.xMin)
			{
				mPointerPosition.x = mBackRect.xMin;
				_timingBarPointerTransform.anchoredPosition = mPointerPosition;
				_timingBarPointerDirection = ETimingBarDirection.Right;
				_isJumpSuccess = false;
			}
			else if (mPointerPosition.x > mBackRect.xMax)
			{
				mPointerPosition.x = mBackRect.xMax;
				_timingBarPointerTransform.anchoredPosition = mPointerPosition;
				_timingBarPointerDirection = ETimingBarDirection.Left;
				_isJumpSuccess = false;
			}
		}

		void CheckJumpCompleteAtSuccessZone()
		{
			Vector2 mPointerPosition = _timingBarPointerTransform.anchoredPosition;
			Rect mSuccessZoneRect = _timingBarSuccessZoneTransform.rect;

			if (_timingBarPointerDirection == ETimingBarDirection.Right)
			{
				if (mPointerPosition.x > mSuccessZoneRect.xMax && !_isJumpSuccess)
				{
					GameOver();
				}
			}
			else if (_timingBarPointerDirection == ETimingBarDirection.Left)
			{
				if (mPointerPosition.x < mSuccessZoneRect.xMin && !_isJumpSuccess)
				{
					GameOver();
				}
			}
		}
		#endregion


		#region Input Func
		void InputTouchScreen()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (IsKeyDownButtonJump(ray))
				{
					_leftCharacter.StartJump();
					CheckJumpAtSuccessZone();
				}

			}
		}

		bool IsKeyDownButtonJump(Ray ray)
		{
			RaycastHit hit;
			if (_jumpButtonCollider.Raycast(ray, out hit, 10.0f))
			{
				return true;
			}
			return false;
		}

		void CheckJumpAtSuccessZone()
		{
			Vector2 mPointerPosition = _timingBarPointerTransform.anchoredPosition;
			Rect mSuccessZoneRect = _timingBarSuccessZoneTransform.rect;

			if (
				mPointerPosition.x > mSuccessZoneRect.xMin &&
				mPointerPosition.x < mSuccessZoneRect.xMax
				)
			{
				_isJumpSuccess = true;
			}
		}
		#endregion

		public void StartGame()
		{
			_gameState = EGameState.Prepare;
		}

		void GameOver()
		{
			_gameState = EGameState.GameOver;

			winnerLabelUi.transform.parent.gameObject.SetActive(true);
		}

	}

}
