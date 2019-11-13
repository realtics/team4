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

    public GameObject popupPanel;
    public GameObject popupGroupYesNo;
    public GameObject popupGroupOk;
    public GameObject popupNotifyText;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void ShowPopup(PopupData data)
    {
        popupPanel.SetActive(true);

        popupGroupOk.SetActive(data.okFlag);
        popupGroupYesNo.SetActive(!data.okFlag);

        popupNotifyText.GetComponent<Text>().text = data.text;
        _popupCallBack = data.callBack;
    }

    public void ClickEventPopup()
    {
        _popupCallBack();
        ClosePopup();
    }

    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

}
