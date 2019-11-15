using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonGameUIManager : Singleton<CommonGameUIManager>
{
	enum ENoticeLabelChildIndex
	{
		SpriteBackground,
		TextWinner,
		DynamicTextUserName
	}

	const string StartGameText = "게임 시작!";

	readonly Vector3 StartGameTimerInitPosition;
	readonly Vector3 ElapseGameTimerInitPosition;

	public delegate void StartGameCallBack();

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
	StartGameCallBack _startGameCallBack;

	float _elapseGameTime;

	void Start()
	{

	}

	#region Start Game Timer
	/// <summary>
	/// 게임 시작전 준비 시간을 나타내주는 UI 호출
	/// </summary>
	/// <param name="canvas">유니티 부모 Canvas 오브젝트</param>
	/// <param name="time">준비 시간</param>
	/// <param name="callBack">타이머가 끝났을때 호출되는 콜백 함수</param>
	public void CreateStartGameTimer(Transform canvas, float time, StartGameCallBack callBack)
	{
		_startGameTimerLabel = Instantiate(prefStartGameTimerLabel, canvas);
		_startGameTimerText = gameObject.GetComponentInChildren<Text>();
		_startGameTime = time;
		_startGameCallBack = callBack;

		_startGameTimerText.text = time.ToString();
	}

	public void UpdateStartGameTimer()
	{
		_startGameTime -= Time.deltaTime;

		if (_startGameTime > 1.0f)
		{
			_startGameTimerText.text = _startGameTime.ToString();
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
	public void CreateElapseGameTimer(Transform canvas)
	{
		_elapseGameTimerLabel = Instantiate(prefElapseGameTimerLabel, canvas);
		_elapseGameTimerText = _elapseGameTimerLabel.GetComponentInChildren<Text>();

		_elapseGameTime = 0.0f;
	}

	public void UpdateElapseGameTimer()
	{
		_elapseGameTime += Time.deltaTime;
		_elapseGameTimerText.text = _elapseGameTime.ToString();
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

}
