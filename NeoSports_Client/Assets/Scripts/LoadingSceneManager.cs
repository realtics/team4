#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FarmGame;

public class LoadingSceneManager : MonoBehaviour
{
	static string nextScene = SceneName.MenuSceneName;

	[SerializeField]
	Image progressBar;

	private void Start()
	{
		switch (nextScene)
		{
			case SceneName.MenuSceneName:
				BundleManager.Instance.Init();
				StartCoroutine(MenuLoadScene());
				break;
			case SceneName.FriendFarmSceneName:
				FriendFarmManager.Instance.RequestFriendFarmData();
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

	IEnumerator MenuLoadScene()
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

				if (progressBar.fillAmount == 0.9f && BundleManager.Instance.IsLoadComplete)
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
