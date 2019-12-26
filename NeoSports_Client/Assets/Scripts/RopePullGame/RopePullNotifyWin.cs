using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
    public class RopePullNotifyWin : MonoBehaviour
    {
        public RopePullGameManager sceneManager;

        void OnTriggerEnter2D(Collider2D other)
        {
			if (other.transform.position.x <= 0)
			{
				sceneManager.NotifyWinner(other.transform, "Left");
			}
			else
			{
				sceneManager.NotifyWinner(other.transform, "Right");
			}
		}

	}
}
