using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePullNotifyWin : MonoBehaviour
{
    public RopePullGameManager sceneManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        sceneManager.NotifyWinner(other.transform);
    }
}
