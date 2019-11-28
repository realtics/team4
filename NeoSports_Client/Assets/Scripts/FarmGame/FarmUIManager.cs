using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmUIManager : Singleton<FarmUIManager>
{

	public enum ECategory
	{
		Default,
		Plant,
		Decoration
	}

	const float DoubleTouchDuration = 0.5f;

	ECategory _currentCategory;

	// Input
	int _clickCount;
	float _touchDuration;
	Touch _touch;

	Camera _mainCamera;
	Farmer _farmer;

	public GameObject prefProductButton;
	public GameObject prefDecorationButton;

	public GameObject plantingPanel;
	public GameObject decorationPanel;

	public GameObject landTileFuncGroup;
	public GameObject objectTileFuncGroup;

	public GameObject productScrollViewContent;
	public GameObject decorationScrollViewContent;

	public GameObject farmerObject;

	void Awake()
	{
		_currentCategory = ECategory.Default;
		_clickCount = 0;
		_touchDuration = 0.0f;
		_touch = new Touch();

		_mainCamera = Camera.main;
		_farmer = farmerObject.GetComponent<Farmer>();

		CreatePlantScrollViewItem();
	}

	void Update()
	{
		if (_currentCategory == ECategory.Default)
		{
			InputClick();
			//InputTouch();
		}
	}

	#region Mouse Logic
	void InputClick()
	{
		if (Input.GetMouseButtonDown(0))
		{
			OnPointerClick();
		}
	}

	void OnPointerClick()
	{
		_clickCount++;
		if (_clickCount == 2)
		{
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				DoubleClickProcess(hit.transform);
			}
			_clickCount = 0;
		}
		else
		{
			StartCoroutine(TickDown());
		}
	}

	private IEnumerator TickDown()
	{
		yield return new WaitForSeconds(DoubleTouchDuration);
		if (_clickCount > 0)
		{
			_clickCount--;
		}
	}
	#endregion

	#region Touch Logic
	void InputTouch()
	{
		if (Input.touchCount > 0)
		{
			_touchDuration += Time.deltaTime;
			_touch = Input.GetTouch(0);

			if (_touch.phase == TouchPhase.Ended && _touchDuration < DoubleTouchDuration - 0.1f)
			{
				StartCoroutine(SingleOrDouble());
			}
		}
		else
		{
			_touchDuration = 0.0f;
		}
	}

	IEnumerator SingleOrDouble()
	{
		yield return new WaitForSeconds(DoubleTouchDuration);
		if (_touch.tapCount == 1)
		{
			Debug.Log("Single");
		}
		else if (_touch.tapCount == 2)
		{
			StopCoroutine(SingleOrDouble());
			Debug.Log("Double");
		}
	}
	#endregion

	void DoubleClickProcess(Transform hitObject)
	{
		if (hitObject.tag == ObjectTag.FarmLand)
		{
			LandTile landTile = hitObject.GetComponent<LandTile>();
			_farmer.SetTargetPosition(landTile);
		}
	}

	void ResetDoubleClick()
	{
		_clickCount = 0;
		_touchDuration = 0.0f;
	}

	#region Button Event
	public void EventBackButton()
	{
		switch (_currentCategory)
		{
			case ECategory.Default:
				ShowExitToMainMenuPopup();
				break;
			case ECategory.Plant:
				ClosePanel(_currentCategory);
				break;
			case ECategory.Decoration:
				ClosePanel(_currentCategory);
				break;
			default:
				break;
		}
	}

	public void EventLandTileFuncButton()
	{
		bool active = landTileFuncGroup.activeInHierarchy;
		objectTileFuncGroup.SetActive(false);
		landTileFuncGroup.SetActive(!active);

		ResetDoubleClick();
	}

	public void EventObjectTileFuncButton()
	{
		bool active = objectTileFuncGroup.activeInHierarchy;
		landTileFuncGroup.SetActive(false);
		objectTileFuncGroup.SetActive(!active);

		ResetDoubleClick();
	}

	public void EventOpenPlantPanelButton()
	{
		_currentCategory = ECategory.Plant;

		objectTileFuncGroup.SetActive(false);
		plantingPanel.SetActive(true);
	}

	public void EventOpenDecorationPanelButton()
	{
		_currentCategory = ECategory.Decoration;

		objectTileFuncGroup.SetActive(false);
		decorationPanel.SetActive(true);
	}

	void ShowExitToMainMenuPopup()
	{
		PopupManager.PopupData data;
		data.text = "메인 메뉴로 나가시겠습니까?";
		data.okFlag = false;
		data.callBack = ChangeSceneToMainMenu;
		PopupManager.Instance.ShowPopup(data);
	}

	void ChangeSceneToMainMenu()
	{
		Debug.Log("Change To Main Menu Scene");
		//SceneManager.LoadScene(SceneName.MenuSceneName);
	}
	#endregion

	public void ClosePanel(ECategory category)
	{
		switch (category)
		{
			case ECategory.Plant:
				_currentCategory = ECategory.Default;
				plantingPanel.SetActive(false);
				break;
			case ECategory.Decoration:
				_currentCategory = ECategory.Default;
				decorationPanel.SetActive(false);
				break;
			default:
				break;
		}
	}

	void CreatePlantScrollViewItem()
	{
		var productDatas = MapData.Instance.ProductDatas;

		foreach (var item in productDatas)
		{
			GameObject obj = Instantiate(prefProductButton, productScrollViewContent.transform);
			obj.GetComponent<ProductButton>().SetData(item.Value);
		}
	}

}
