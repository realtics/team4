using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PopupManager>
{

    public enum EPanel
    {
        Main,
        Play,
        Gallary,
        Option,
        Credit
    }

    Dictionary<EPanel, GameObject> panelDic;
    EPanel currentPanel;

    public GameObject panelMain;
    public GameObject panelPlay;
    public GameObject panelGallary;
    public GameObject panelOption;
    public GameObject panelCredit;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        InitPanelList();

        EnablePanel(EPanel.Main);
    }

    void InitPanelList()
    {
        panelDic = new Dictionary<EPanel, GameObject>();

        panelDic.Add(EPanel.Main, panelMain);
        panelDic.Add(EPanel.Play, panelPlay);
        panelDic.Add(EPanel.Gallary, panelGallary);
        panelDic.Add(EPanel.Option, panelOption);
        panelDic.Add(EPanel.Credit, panelCredit);
    }

    void EnablePanel(EPanel type)
    {
        foreach (KeyValuePair<EPanel, GameObject> item in panelDic)
        {
            if(item.Value == null)
            {
                continue;
            }
            if (item.Key == type)
            {
                item.Value.SetActive(true);
            }
            else
            {
                item.Value.SetActive(false);
            }
        }

        currentPanel = type;
    }


    #region Click Event - Main
    public void ClickEventButtonPlay()
    {
        EnablePanel(EPanel.Play);
    }

    public void ClickEventButtonGallary()
    {
        EnablePanel(EPanel.Gallary);
    }

    public void ClickEventButtonOption()
    {
        EnablePanel(EPanel.Option);
    }

    public void ClickEventButtonCredit()
    {
        EnablePanel(EPanel.Credit);
    }

    public void ClickEventButtonBack()
    {
        switch (currentPanel)
        {
            case EPanel.Main:
                PopupManager.PopupData data;
                data.text = "게임을 종료하시겠습니까?";
                data.okFlag = false;
                data.callBack = ExitProgram;
                Singleton<PopupManager>.Instance.ShowPopup(data);
                break;
            default:
                EnablePanel(EPanel.Main);
                break;
        }
    }
    #endregion

    void ExitProgram()
    {
        Debug.Log("Call Exit");
        Application.Quit();
    }

}
