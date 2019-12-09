using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class NickNameManager : Singleton<NickNameManager>
	{
		const int MaxNickName = 8;

		public GameObject nickNameInputGroup;
		public GameObject nickNameInputText;

		private void Start()
		{
			if (InventoryManager.Instance.IsNickNameDecide)
			{
				nickNameInputGroup.SetActive(false);
			}
		}

		public void DecideNickName()
		{
			string inputName = nickNameInputText.transform.GetComponent<Text>().text;
			if (!IsUseableNickName(inputName))
			{
				return;
			}

			InventoryManager.Instance.PlayerNickName = inputName;
			nickNameInputGroup.SetActive(false);
			NetworkManager.Instance.SendNickName(inputName);
			InventoryManager.Instance.IsNickNameDecide = true;
			Debug.Log("결정된 닉네임: " + inputName);
		}

		bool IsUseableNickName(string name)
		{
			if (name.Length == 0)
			{
				PopupManager.PopupData pData;
				pData.text = "최소 1글자 이상은 입력해야 합니다!";
				pData.okFlag = true;
				pData.callBack = null;
				PopupManager.Instance.ShowPopup(pData);
				return false;
			}

			if (name.Length > MaxNickName)
			{
				PopupManager.PopupData pData;
				pData.text = "닉네임은 최대 8글자 까지 가능합니다!";
				pData.okFlag = true;
				pData.callBack = null;
				PopupManager.Instance.ShowPopup(pData);
				return false;
			}

			return true;
		}
	}
}
