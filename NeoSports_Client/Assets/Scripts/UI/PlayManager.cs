using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayManager : Singleton<PlayManager>
{


	private void Awake()
	{
		DontDestroyOnLoad(this);
	}

	public void StartRopeGame()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName.RopeGameSceneName);
	}

}
