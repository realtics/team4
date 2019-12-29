using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Intro : MonoBehaviour
{
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			LoadingSceneManager.LoadScene(SceneName.MenuSceneName);
		}
	}

}
