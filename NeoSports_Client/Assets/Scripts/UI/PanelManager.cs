#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{

    public enum EPanel
    {
        Main,
        Play,
        Gallary,
        Option,
        Credit
    }

    Dictionary<EPanel, GameObject> _panelDic;
    EPanel _currentPanel;

	[SerializeField]
    GameObject panelMain;
	[SerializeField]
	GameObject panelPlay;
	[SerializeField]
	GameObject panelGallary;
	[SerializeField]
	GameObject panelOption;
	[SerializeField]
	GameObject panelCredit;
	[SerializeField]
	GameObject otherFarmPopupGroup;


	void Start()
	{
		InitPanelList();

		EnablePanel(EPanel.Main);
	}

	void InitPanelList()
    {
		_panelDic = new Dictionary<EPanel, GameObject>
		{
			{ EPanel.Main, panelMain },
			{ EPanel.Play, panelPlay },
			{ EPanel.Gallary, panelGallary },
			{ EPanel.Option, panelOption },
			{ EPanel.Credit, panelCredit }
		};
	}

    void EnablePanel(EPanel type)
    {
        foreach (KeyValuePair<EPanel, GameObject> item in _panelDic)
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

        _currentPanel = type;
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
		if (otherFarmPopupGroup.activeInHierarchy)
		{
			otherFarmPopupGroup.SetActive(false);
			return;
		}

        switch (_currentPanel)
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
