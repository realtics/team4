using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{

    public delegate void PopupCallBack();

    public struct PopupData
    {
        public string text;
        public bool okFlag;
        public PopupCallBack callBack;
    }

    PopupCallBack _popupCallBack;
    Text _popupNotifyText;

    public GameObject popupPanel;
    public GameObject popupNotifyText;
    public GameObject popupGroupYesNo;
    public GameObject popupGroupOk;

    void Awake()
    {
        _popupNotifyText = popupNotifyText.GetComponent<Text>();

        if (popupPanel.activeInHierarchy)
        {
            popupPanel.SetActive(false);
        }
    }

    public void ShowPopup(PopupData data)
    {
        popupPanel.SetActive(true);

        popupGroupOk.SetActive(data.okFlag);
        popupGroupYesNo.SetActive(!data.okFlag);

        _popupNotifyText.text = data.text;
        _popupCallBack = data.callBack;

		SetAsLastSiblingPopup();

	}

	public void SetAsLastSiblingPopup()
	{
		popupPanel.transform.SetAsLastSibling();
	}

    public void ClickEventPopup()
    {
        _popupCallBack?.Invoke();
        ClosePopup();
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

}
