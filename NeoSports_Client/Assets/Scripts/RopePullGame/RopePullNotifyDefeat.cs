using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
    public class RopePullNotifyDefeat : MonoBehaviour
    {
        public RopePullGameManager sceneManager;

        void OnTriggerEnter2D(Collider2D other)
        {
            sceneManager.NotifyLoser(other.transform);
        }
    }
}
