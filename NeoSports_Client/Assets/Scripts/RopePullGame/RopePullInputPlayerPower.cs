using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePullGame
{
	public class RopePullInputPlayerPower : MonoBehaviour
	{
		public float pullPower;
		public float strength;
		public Action OutlineEffect;

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
					pullPower += strength;
					OutlineEffect();
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
			pullPower = 0.0f;
		}
	}
}
