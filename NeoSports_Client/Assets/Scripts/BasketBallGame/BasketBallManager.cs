using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasketBallGame
{
    public class BasketBallManager : MonoBehaviour
    {
        public GameObject ballManager;
        public float playTimerLimit;

        float _playTime;

        void Start()
        {
            StartCoroutine(SecondCount());
        }

        IEnumerator SecondCount()
        {
            while (true)
            {
                _playTime += 1.0f;

                if (_playTime >= playTimerLimit)
                {
                    EndPlayGame();
                    yield break;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        void EndPlayGame()
        {
            //GameState값 End. 
        }

    }
}
