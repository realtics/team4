using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpRopeGame
{
	public class BackgroundManager : Singleton<BackgroundManager>
	{
		const string PrefabCloudPath = "Prefabs/JumpRopeGame/Prefab_Cloud";

		const float GenerateCloudIntervalMin = 1.0f;
		const float GenerateCloudIntervalMax = 5.0f;

		float GenerateCloudCycle = 0.0f;
		float GenerateCloudTimer = 0.0f;

		GameObject prefCloud;

		public GameObject BackgroundCloudGroup;

		#region Engine Call
		private void Awake()
		{
			prefCloud = Resources.Load<GameObject>(PrefabCloudPath);
		}

		private void Start()
		{

		}

		private void Update()
		{
			GenerateCloud();
		}
		#endregion

		void GenerateCloud()
		{
			GenerateCloudTimer += Time.deltaTime;

			if (GenerateCloudTimer > GenerateCloudCycle)
			{
				GenerateCloudTimer -= GenerateCloudCycle;
				GenerateCloudCycle = Random.Range(GenerateCloudIntervalMin, GenerateCloudIntervalMax);
				Instantiate(prefCloud, BackgroundCloudGroup.transform);
			}
		}

	}
}

