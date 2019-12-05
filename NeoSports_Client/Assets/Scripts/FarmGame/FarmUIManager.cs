using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace FarmGame
{
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

			_mainCamera = Camera.main;
			_farmer = farmerObject.GetComponent<Farmer>();

			CreatePlantScrollViewItem();
			CreateDecorationScrollViewItem();
		}

		void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				if (Input.GetMouseButtonDown(0))
				{
					InputClick();
				}
			}
		}

		void InputClick()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 10.0f))
			{
				if (hit.transform.tag == ObjectTag.FarmLand)
				{
					LandTile landTile = hit.transform.GetComponent<LandTile>();
					_farmer.SetTargetPosition(landTile);
				}
			}
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
		}

		public void EventObjectTileFuncButton()
		{
			bool active = objectTileFuncGroup.activeInHierarchy;
			landTileFuncGroup.SetActive(false);
			objectTileFuncGroup.SetActive(!active);
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

		void CreateDecorationScrollViewItem()
		{
			var decorationDatas = MapData.Instance.DecorationDatas;

			foreach (var item in decorationDatas)
			{
				GameObject obj = Instantiate(prefDecorationButton, decorationScrollViewContent.transform);
				obj.GetComponent<DecorationButton>().SetData(item.Value);
			}
		}

	}

}
