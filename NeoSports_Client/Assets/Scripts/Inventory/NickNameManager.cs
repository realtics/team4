using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
	public class NickNameManager : Singleton<NickNameManager>
	{
		const int MaxNickName = 8;

		[SerializeField]
		GameObject nickNameInputGroup;
		[SerializeField]
		InputField nickNameInputField;
		[SerializeField]
		Text nickNameInputText;

		string _nickName;

		private void Awake()
		{
			CheckSaveNickName();
		}

		private void Start()
		{
			if (InventoryManager.Instance.IsNickNameDecide)
			{
				nickNameInputGroup.SetActive(false);
			}
			else
			{
				InventoryManager.Instance.PlayerNickName = _nickName;
			}
		}

		void CheckSaveNickName()
		{
			_nickName = PlayerPrefs.GetString(PrefsKey.NickNameKey, "플레이어");
			Debug.Log(_nickName);
			nickNameInputField.text = _nickName;
			nickNameInputText.text = _nickName;
		}

		public void DecideNickName()
		{
			_nickName = nickNameInputText.text;
			if (!IsUseableNickName())
			{
				return;
			}

			PlayerPrefs.SetString(PrefsKey.NickNameKey, _nickName);
			InventoryManager.Instance.PlayerNickName = _nickName;
			nickNameInputGroup.SetActive(false);
			NetworkManager.Instance.SendNickName(_nickName);
			InventoryManager.Instance.IsNickNameDecide = true;

			Debug.Log("결정된 닉네임: " + _nickName);
		}

		bool IsUseableNickName()
		{
			if (_nickName.Length == 0)
			{
				PopupManager.PopupData pData;
				pData.text = "최소 1글자 이상은 입력해야 합니다!";
				pData.okFlag = true;
				pData.callBack = null;
				PopupManager.Instance.ShowPopup(pData);
				return false;
			}

			if (_nickName.Length > MaxNickName)
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
