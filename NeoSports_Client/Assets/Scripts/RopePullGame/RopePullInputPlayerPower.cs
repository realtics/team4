using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
    public class RopePullInputPlayerPower : MonoBehaviour
    {
        [HideInInspector]
        public float PullPower { get; set; }
        public float strength;
        public GameObject tapParticleSystem;

        void Update()
        {
            UpdateInput();
        }

        void UpdateInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
                PullPower += strength;
            }
        }

        public void ResetPower()
        {
            PullPower = 0.0f;
        }
    }
}
