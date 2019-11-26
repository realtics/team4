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

		bool _isSinglePlay;
		void Start()
		{
			_isSinglePlay = NetworkManager.Instance.IsSinglePlay();
		}

		void Update()
        {
            UpdateInput();
        }

        void UpdateInput()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
            {
				if (_isSinglePlay)
				{
					PullPower += strength;
				}
				else
				{
					NetworkManager.Instance.SendRequestRopePull(PullPower);
				}
            }
        }

        public void ResetPower()
        {
            PullPower = 0.0f;
        }
    }
}
