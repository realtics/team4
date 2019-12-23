#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FarmGame;

public class PlayManager : Singleton<PlayManager>
{

	[SerializeField]
	GameObject prefFriendFarmManager;
	[SerializeField]
	GameObject enterFriendFarmGroup;
	[SerializeField]
	Text otherFarmUIDText;

	int friendFarmClientId = 0;

	public void ButtonEvent_StartRopePullSingleGame()
	{
		SceneManager.LoadScene(SceneName.RopeGameSceneName);
	}

	public void ButtonEvent_StartRopePullMultiGame()
	{
		NetworkManager.Instance.SendRequestRoom(GAME_INDEX.ROPE_PULL);
		SceneManager.LoadSceneAsync(SceneName.NetworkRopeGameSceneName);
	}

	public void ButtonEvent_StartJumpRopeSingleGame()
	{
		SceneManager.LoadScene(SceneName.JumpRopeGameSceneName);
	}

	public void ButtonEvent_StartJumpRopeMultiGame()
	{
        NetworkManager.Instance.SendRequestRoom(GAME_INDEX.ROPE_JUMP);
    }

	public void ButtonEvent_StartBasketBallSingleGame()
	{
		SceneManager.LoadScene(SceneName.BasketBallGameSceneName);
	}

	public void ButtonEvent_StartBasketBallMultiGame()
	{
        NetworkManager.Instance.SendRequestRoom(GAME_INDEX.BASKET_BALL);
    }

	public void ButtonEvent_StartFarmScene()
	{
		SceneManager.LoadScene(SceneName.FarmSceneName);
	}

	public void ButtonEvent_EnterOtherFarmUID()
	{
		string uidStr = otherFarmUIDText.text;
		int uidNum;
		if(uidStr != string.Empty && int.TryParse(uidStr, out uidNum))
		{
			friendFarmClientId = uidNum;
			NetworkManager.Instance.SendFriendFarmDataRequest(uidNum);
		}
		enterFriendFarmGroup.SetActive(false);
	}

	public void ProcessClientIdCheckResult(bool isExist)
	{
		if (isExist)
		{
			Instantiate(prefFriendFarmManager);
			FriendFarmManager.Instance.FriendFarmClientId = friendFarmClientId;
			LoadingSceneManager.LoadScene(SceneName.FriendFarmSceneName);
		}
		else
		{
			PopupManager.PopupData data;
			data.okFlag = true;
			data.text = "존재하지 않는 농장입니다.";
			data.callBack = null;
			PopupManager.Instance.ShowPopup(data);
		}
	}

}
