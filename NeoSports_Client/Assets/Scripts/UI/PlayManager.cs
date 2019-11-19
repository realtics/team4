using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayManager : Singleton<PlayManager>
{

	public GameObject ropePullMultiButton;
	public GameObject ropeJumpMultiButton;
	public GameObject basketBallMultiButton;

	void Start()
	{
		DisableMultiPlayButton();
	}

	void DisableMultiPlayButton()
	{
		ropePullMultiButton.GetComponent<Button>().enabled = false;
		ropeJumpMultiButton.GetComponent<Button>().enabled = false;
		basketBallMultiButton.GetComponent<Button>().enabled = false;
	}

	#region Button Event
	public void StartRopePullSingleGame()
	{
		SceneManager.LoadScene(SceneName.RopeGameSceneName);
	}

	public void StartRopePullMultiGame()
	{
		SceneManager.LoadScene(SceneName.RopeGameSceneName);
	}

	public void StartJumpRopeSingleGame()
	{
		SceneManager.LoadScene(SceneName.JumpRopeGameSceneName);
	}

	public void StartJumpRopeMultiGame()
	{
		SceneManager.LoadScene(SceneName.JumpRopeGameSceneName);
	}

	public void StartBasketBallSingleGame()
	{
		SceneManager.LoadScene(SceneName.BasketBallGameSceneName);
	}

	public void StartBasketBallMultiGame()
	{
		SceneManager.LoadScene(SceneName.BasketBallGameSceneName);
	}
	#endregion

}
