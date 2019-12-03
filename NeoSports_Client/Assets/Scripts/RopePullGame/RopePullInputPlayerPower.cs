using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
	public class RopePullInputPlayerPower : MonoBehaviour
	{
		public float PullPower;
		public float strength;

		private bool _isOwnHost;

		bool _isSinglePlay;
		void Start()
		{
			_isSinglePlay = NetworkManager.Instance.IsSinglePlay();
			_isOwnHost = NetworkManager.Instance.isOwnHost;
		}

		void Update()
		{
			UpdateInput();
		}

		void UpdateInput()
		{
			if (RopePullGameManager.Instance.SceneState != RopePullGameManager.ESceneState.Play)
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
			{
				if (_isSinglePlay)
				{
					PullPower += strength;
				}
				else
				{
					if (_isOwnHost)
						NetworkManager.Instance.SendRequestRopePull(strength * -1);
					else
						NetworkManager.Instance.SendRequestRopePull(strength);
				}
			}
		}

		public void ResetPower()
		{
			PullPower = 0.0f;
		}
	}
}
