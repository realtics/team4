﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayManager : Singleton<PlayManager>
{

	public GameObject ropePullMultiButton;
	public GameObject ropeJumpMultiButton;
	public GameObject basketBallMultiButton;

	public void ButtonEvent_StartRopePullSingleGame()
	{
		SceneManager.LoadScene(SceneName.RopeGameSceneName);
	}

	public void ButtonEvent_StartRopePullMultiGame()
	{
		SceneManager.LoadSceneAsync(SceneName.NetworkRopeGameSceneName);
	}

	public void ButtonEvent_StartJumpRopeSingleGame()
	{
		SceneManager.LoadScene(SceneName.JumpRopeGameSceneName);
	}

	public void ButtonEvent_StartJumpRopeMultiGame()
	{
        NetworkManager.Instance.SendRequestRoom(GAME_INDEX.ROPE_JUMP);
    }

	public void ButtonEvent_StartBasketBallSingleGame()
	{
		SceneManager.LoadScene(SceneName.BasketBallGameSceneName);
	}

	public void ButtonEvent_StartBasketBallMultiGame()
	{
        NetworkManager.Instance.SendRequestRoom(GAME_INDEX.BASKET_BALL);
    }

	public void ButtonEvent_StartFarmScene()
	{
		SceneManager.LoadScene(SceneName.FarmSceneName);
	}

}
