using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : Singleton<BackgroundManager>
{
	const float GenerateCloudIntervalMin = 1.0f;
	const float GenerateCloudIntervalMax = 5.0f;

	float GenerateCloudCycle = 0.0f;
	float GenerateCloudTimer = 0.0f;

	public GameObject prefCloud;
	public GameObject BackgroundCloudGroup;

	private void Update()
	{
		GenerateCloud();
	}

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
