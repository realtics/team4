using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
    public class RopePulllAIPlayerPower : MonoBehaviour
    {
        [HideInInspector]
        public float PullPower { get; set; }

        public float stength;
        public float difficultyLevel;
        public float updateAICycleTime;

        void Start()
        {
            StartCoroutine(SecondCount());
        }

        IEnumerator SecondCount()
        {
            while (true)
            {
                PullPower += stength * difficultyLevel;
                yield return new WaitForSeconds(updateAICycleTime);
            }
        }

        public void ResetPower()
        {
            PullPower = 0.0f;
        }
    }
}
