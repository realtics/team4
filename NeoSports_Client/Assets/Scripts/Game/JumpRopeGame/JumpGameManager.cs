using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	const float		GameStartPrepareTime = 5.0f;
	const string	GameStartPrepareTimeStartText = "GAME START!";

	const float		TimingBarPointerInitialSpeed = 150.0f;
	#endregion

	#region public Variable
	// UI Object
	public GameObject UITimingBarBack;
	public GameObject UITimingBarSuccessZone;
	public GameObject UITimingBarPointer;

	public GameObject UIStartGameTimer;
	public GameObject UIElapseGameTimer;

	public GameObject UIWinnerLabel;

	public GameObject UIJumpButton;

	// Game Object
	public GameObject leftPlayer;
	#endregion

	#region Private Variable
	EGameState	_gameState;
	Character	_leftCharacter;

	// Timing Bar
	RectTransform _timingBarBackTransform;
	RectTransform _timingBarSuccessZoneTransform;
	RectTransform _timingBarPointerTransform;

	ETimingBarDirection _timingBarPointerDirection;
	float	_timingBarPointerSpeed;
	bool	_isJumpSuccess;

	// Timer
	float _startGameTimer;
	float _elpaseGameTimer;
	#endregion

	private void Start()
	{
		_gameState = EGameState.PlayerWait;
		_leftCharacter = leftPlayer.transform.GetChild(0).GetComponent<Character>();

		_timingBarBackTransform = UITimingBarBack.GetComponent<RectTransform>();
		_timingBarSuccessZoneTransform = UITimingBarSuccessZone.GetComponent<RectTransform>();
		_timingBarPointerTransform = UITimingBarPointer.GetComponent<RectTransform>();

		_timingBarPointerDirection = ETimingBarDirection.Right;
		_timingBarPointerSpeed = TimingBarPointerInitialSpeed;
		_isJumpSuccess = false;

		_startGameTimer = GameStartPrepareTime;
		_elpaseGameTimer = 0.0f;

		StartGame();
	}

	private void Update()
	{
		switch(_gameState)
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

	public void StartGame()
	{
		_gameState = EGameState.Prepare;
	}

	#region Update Timer
	void UpdateStartGameTimer()
	{
		_startGameTimer -= Time.deltaTime;

		Text mStartGameTimerText = UIStartGameTimer.GetComponent<Text>();
		if (_startGameTimer > 1.0f)
		{
			mStartGameTimerText.text = ((int)_startGameTimer).ToString();
		}
		else if (_startGameTimer > 0.0f)
		{
			mStartGameTimerText.text = GameStartPrepareTimeStartText;
		}
		else
		{
			_gameState = EGameState.Playing;

			GameObject mUIStartGameTimerParent = UIStartGameTimer.transform.parent.gameObject;
			mUIStartGameTimerParent.SetActive(false);
			UIElapseGameTimer.SetActive(true);
		}
	}

	void UpdateElapseGameTimer()
	{
		_elpaseGameTimer += Time.deltaTime;

		Text mElapseGameTimerText = UIElapseGameTimer.GetComponent<Text>();
		mElapseGameTimerText.text = ((int)_elpaseGameTimer).ToString();
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
			if(mPointerPosition.x < mSuccessZoneRect.xMin && !_isJumpSuccess)
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
		if (UIJumpButton.GetComponent<BoxCollider>().Raycast(ray, out hit, 10.0F))
		{
			return true;
		}
		return false;
	}

	void CheckJumpAtSuccessZone()
	{
		Vector2 mPointerPosition = _timingBarPointerTransform.anchoredPosition;
		Rect mSuccessZoneRect = _timingBarSuccessZoneTransform.rect;

		if(
			mPointerPosition.x > mSuccessZoneRect.xMin &&
			mPointerPosition.x < mSuccessZoneRect.xMax
			)
		{
			_isJumpSuccess = true;
		}
	}
	#endregion

	void GameOver()
	{
		_gameState = EGameState.GameOver;

		UIWinnerLabel.transform.parent.gameObject.SetActive(true);
	}

}
