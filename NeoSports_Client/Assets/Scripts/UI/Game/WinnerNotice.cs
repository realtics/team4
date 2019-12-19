#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerNotice : MonoBehaviour
{
	[SerializeField]
	Text userNameText;
	[SerializeField]
	Text rewardGoldText;

	public void SetUserNameText(string name)
	{
		userNameText.text = name;
	}

	public void SetRewardGoldText(int amount)
	{
		rewardGoldText.text = amount.ToString("N0");
	}
}
