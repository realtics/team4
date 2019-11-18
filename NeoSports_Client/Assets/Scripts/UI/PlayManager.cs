using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManager : Singleton<PlayManager>
{

	public void StartRopePullGame()
	{
		SceneManager.LoadScene(SceneName.RopeGameSceneName);
	}

	public void StartJumpRopeGame()
	{
		SceneManager.LoadScene(SceneName.JumpRopeGameSceneName);
	}

}
