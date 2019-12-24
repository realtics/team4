#pragma warning disable CS0649
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
		GameObject nickNameInputPanel;
		[SerializeField]
		InputField nickNameInputField;
		[SerializeField]
		Text nickNameInputText;
		[SerializeField]
		Text userIdInfoText;

		string _nickName;

		private void Start()
		{
			CheckSaveNickName();
		}

		public void CheckSaveNickName()
		{
			if (PlayerPrefs.HasKey(PrefsKey.ClientIdKey))
			{
				_nickName = PlayerPrefs.GetString(PrefsKey.NickNameKey, "플레이어");
				InventoryManager.Instance.PlayerNickName = _nickName;
				//NetworkManager.Instance.SendNickName(_nickName);
				SetUserIdInfoText(PlayerPrefs.GetInt(PrefsKey.ClientIdKey));
			}
			else
			{
				nickNameInputPanel.SetActive(true);
			}
		}

		public void ButtonEvent_OpenNickNamePanel()
		{
			nickNameInputPanel.SetActive(true);
			nickNameInputField.text = _nickName;
			nickNameInputText.text = _nickName;
		}

		public void ButtonEvent_DecideNickName()
		{
			_nickName = nickNameInputText.text;
			if (!IsUseableNickName())
			{
				return;
			}

			nickNameInputPanel.SetActive(false);

			if (!PlayerPrefs.HasKey(PrefsKey.NickNameKey))
			{
				ShowNickNameCanChangePopup();
			}

			PlayerPrefs.SetString(PrefsKey.NickNameKey, _nickName);
			InventoryManager.Instance.PlayerNickName = _nickName;
			NetworkManager.Instance.SendNickName(_nickName);

			Debug.Log("결정된 닉네임: " + _nickName);
		}

		public void Debug_ClearNickName()
		{
			PlayerPrefs.DeleteKey(PrefsKey.NickNameKey);
			PlayerPrefs.DeleteKey(PrefsKey.ClientIdKey);
		}

		void ShowNickNameCanChangePopup()
		{
			PopupManager.PopupData data;
			data.callBack = null;
			data.okFlag = true;
			data.text = "닉네임은 옵션에서 언제든지 변경할 수 있습니다!";

			PopupManager.Instance.ShowPopup(data);
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

		public void SetUserIdInfoText(int userId)
		{
			userIdInfoText.text = userId.ToString();
		}
	}
}
