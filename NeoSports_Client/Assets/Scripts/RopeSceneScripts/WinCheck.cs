using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCheck : MonoBehaviour
{
    // Start is called before the first frame update
    RopePullGameManager sceneManager;
    void Start()
    {
        sceneManager = GameObject.FindWithTag("SceneManager").GetComponent<RopePullGameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        sceneManager.NotifyWinner(other);
    }
}
