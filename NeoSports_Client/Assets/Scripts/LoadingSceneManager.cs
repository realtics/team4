#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FarmGame;

public class LoadingSceneManager : MonoBehaviour
{
	static string nextScene;

	[SerializeField]
	Image progressBar;
	[SerializeField]
	GameObject prefFriendFarmManager;

	private void Start()
	{
		switch (nextScene)
		{
			case SceneName.FriendFarmSceneName:
				Instantiate(prefFriendFarmManager);
				StartCoroutine(FriendFarmLoadScene());
				break;
			default:
				StartCoroutine(DefaultLoadScene());
				break;
		}
	}

	public static void LoadScene(string sceneName)
	{
		nextScene = sceneName;
		SceneManager.LoadScene(SceneName.FriendFarmLoadingSceneName);
	}

	IEnumerator DefaultLoadScene()
	{
		yield return null;

		AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
		op.allowSceneActivation = false;

		float timer = 0.0f;

		while (!op.isDone)
		{
			yield return null;

			timer += Time.deltaTime;

			if (op.progress < 0.9f)
			{
				progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
				if (progressBar.fillAmount >= op.progress)
				{
					timer = 0f;
				}
			}
			else
			{
				progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 0.9f, timer);

				if (progressBar.fillAmount == 0.9f && FriendFarmManager.Instance.IsDataLoaded)
				{
					progressBar.fillAmount = 1.0f;
					op.allowSceneActivation = true;
					yield break;
				}
			}
		}
	}

	IEnumerator FriendFarmLoadScene()
	{
		yield return null;

		AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
		op.allowSceneActivation = false;

		float timer = 0.0f;

		while (!op.isDone)
		{
			yield return null;

			timer += Time.deltaTime;

			if (op.progress < 0.9f)
			{
				progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
				if (progressBar.fillAmount >= op.progress)
				{
					timer = 0f;
				}
			}
			else
			{
				progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 0.9f, timer);

				if (progressBar.fillAmount == 0.9f && FriendFarmManager.Instance.IsDataLoaded)
				{
					progressBar.fillAmount = 1.0f;
					op.allowSceneActivation = true;
					yield break;
				}
			}
		}

	}

}
