using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Intro : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
		LoadingSceneManager.LoadScene(SceneName.MenuSceneName);
    }

}
