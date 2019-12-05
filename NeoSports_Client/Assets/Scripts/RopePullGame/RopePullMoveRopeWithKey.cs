using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
    public class RopePullMoveRopeWithKey : Singleton<RopePullMoveRopeWithKey>
    {
        public bool IsStart { get; set; }
        float Speed { get; set; }
        float powerSum;
        float feverPower;

        public GameObject LeftPlayer;
        public GameObject RightPlayer;

		bool isSinglePlay;

        void Start()
        {
            IsStart = false;
            Speed = 0.0f;
            powerSum = 0.0f;
            feverPower = 1.0f;
			if (NetworkManager.Instance != null)
			{
				isSinglePlay = NetworkManager.instance.IsSinglePlay();
			}
        }

        void Update()
        {
            UpdateKey();
            UpdateRope();
        }

        public void UpdateKey()
        {
            if (IsStart)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    transform.Translate(new Vector3(-1 * Speed * Time.deltaTime, 0f, 0f));
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    transform.Translate(new Vector3(1 * Speed * Time.deltaTime, 0f, 0f));
                }
            }
        }

        void UpdateRope()
        {
            if (IsStart)
            {
                GetPlayersPower();
				if (isSinglePlay)
				{
					UpdateRopePosition();
				}
            }
            LeftPlayer.GetComponent<RopePullInputPlayerPower>().ResetPower();
            RightPlayer.GetComponent<RopePulllAIPlayerPower>().ResetPower();
        }

        void CalculateRopeMove(float leftPower, float rightPower)
        {
            powerSum = rightPower - leftPower;
        }

        void UpdateRopePosition()
        {
            transform.Translate(new Vector3(feverPower * powerSum * Time.deltaTime, 0f, 0f));
        }

		public void UpdateNetworkRopePostion(float pullPower)
		{
			//transform.Translate(new Vector3(pullPower * Time.deltaTime, 0f, 0f));
			//TEST
			transform.SetPositionAndRotation(new Vector3(pullPower * Time.deltaTime, 0f, 0f),Quaternion.identity);
		}

        void GetPlayersPower()
        {
            float leftPower = LeftPlayer.GetComponent<RopePullInputPlayerPower>().pullPower;
            float RightPower = RightPlayer.GetComponent<RopePulllAIPlayerPower>().PullPower;
            CalculateRopeMove(leftPower, RightPower);
        }

        public void SetFeverTime()
        {
            feverPower = 4.0f;
        }
        public void ResetFeverTime()
        {
            feverPower = 1.0f;
        }
    }
}
