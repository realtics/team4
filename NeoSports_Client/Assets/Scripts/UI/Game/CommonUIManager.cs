﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CommonUIManager : Singleton<CommonUIManager>
{
	enum ENoticeLabelChildIndex
	{
		SpriteBackground,
		TextWinner,
		DynamicTextUserName
	}

	public delegate void StartGameCallBack();

	const string StartGameText = "게임 시작!";
	const string SureToChangeMainMenuScene = "메인 메뉴로 나가시겠습니까?";
	readonly Vector2 ElapseGameTimerInitPosition = new Vector2(0, 190);

	public GameObject prefStartGameTimerLabel;
	public GameObject prefElapseGameTimerLabel;

	public GameObject prefWinnerNoticeLabel;
	public GameObject prefLooserNoticeLabel;

	public GameObject prefBackButton;

	GameObject	_startGameTimerLabel;
	GameObject _elapseGameTimerLabel;

	GameObject _winnerNoticeLabel;
	GameObject _looserNoticeLabel;

	Text _startGameTimerText;
	Text _elapseGameTimerText;

	float _startGameTime;
	float _elapseGameTime;
	StartGameCallBack _startGameCallBack;


	#region Start Game Timer
	/// <summary>
	/// 게임 시작전 준비 시간을 나타내주는 UI 호출
	/// </summary>
	/// <param name="canvas">유니티 부모 Canvas 오브젝트</param>
	/// <param name="time">준비 시간</param>
	/// <param name="callBack">타이머가 끝났을때 호출되는 콜백 함수</param>
	public void CreateStartGameTimer(GameObject canvas, float time, StartGameCallBack callBack)
	{
		_startGameTimerLabel = Instantiate(prefStartGameTimerLabel, canvas.transform);
		_startGameTimerText = _startGameTimerLabel.GetComponentInChildren<Text>();
		_startGameTime = time;
		_startGameCallBack = callBack;

		_startGameTimerText.text = time.ToString();
	}

	public void UpdateStartGameTimer()
	{
		_startGameTime -= Time.deltaTime;

		if (_startGameTime > 1.0f)
		{
			_startGameTimerText.text = ((int)_startGameTime).ToString();
		}
		else if (_startGameTime > 0.0f)
		{
			_startGameTimerText.text = StartGameText;
		}
		else
		{
			_startGameCallBack();
			Destroy(_startGameTimerLabel);
		}
	}
	#endregion

	#region Elapse Game Timer
	/// <summary>
	/// 게임 시작 후 흐른 시간을 보여주는 UI 호출
	/// </summary>
	/// <param name="canvas">유니티 부모 Canvas 오브젝트</param>
	public void CreateElapseGameTimer(GameObject canvas)
	{
		_elapseGameTimerLabel = Instantiate(prefElapseGameTimerLabel, canvas.transform);
		_elapseGameTimerText = _elapseGameTimerLabel.GetComponentInChildren<Text>();

		_elapseGameTime = 0.0f;
		_elapseGameTimerText.text = ((int)_elapseGameTime).ToString();
		_elapseGameTimerLabel.GetComponent<RectTransform>().anchoredPosition = ElapseGameTimerInitPosition;
	}

	public void UpdateElapseGameTimer()
	{
		_elapseGameTime += Time.deltaTime;
		_elapseGameTimerText.text = ((int)_elapseGameTime).ToString();
	}

	public void DestroyElapseGameTimer()
	{
		Destroy(_elapseGameTimerLabel);
	}
	#endregion

	#region Game Result Notice Label
	/// <summary>
	/// 게임 승리자를 보여주는 UI 호출
	/// </summary>
	/// <param name="canvas">유니티 부모 Canvas 오브젝트</param>
	/// <param name="name">승리 유저 이름</param>
	public void CreateWinnerNotice(Transform canvas, string name)
	{
		_winnerNoticeLabel = Instantiate(prefWinnerNoticeLabel, canvas);
		_winnerNoticeLabel.transform.GetChild((int)ENoticeLabelChildIndex.DynamicTextUserName).
			GetComponent<Text>().text = name;
	}

	public void DestroyWinnerNotice()
	{
		Destroy(_winnerNoticeLabel);
	}

	/// <summary>
	/// 게임 패배자를 보여주는 UI 호출
	/// </summary>
	/// <param name="canvas">유니티 부모 Canvas 오브젝트</param>
	/// <param name="name">패배 유저 이름</param>
	public void CreateLooserNotice(Transform canvas, string name)
	{
		_looserNoticeLabel = Instantiate(prefLooserNoticeLabel, canvas);
		_looserNoticeLabel.transform.GetChild((int)ENoticeLabelChildIndex.DynamicTextUserName).
			GetComponent<Text>().text = name;
	}

	public void DestroyLooserNotice()
	{
		Destroy(_looserNoticeLabel);
	}
	#endregion

	public void ShowExitPopup()
	{
		PopupManager.PopupData data;
		data.text = SureToChangeMainMenuScene;
		data.okFlag = false;
		data.callBack = SceneChangeToMainMenu;
		PopupManager.Instance.ShowPopup(data);
	}

	void SceneChangeToMainMenu()
	{
		SceneManager.LoadScene(SceneName.MenuSceneName);
	}

}
