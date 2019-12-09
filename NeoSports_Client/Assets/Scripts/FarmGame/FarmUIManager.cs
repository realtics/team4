using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
			Decoration,
			Storage
		}

		ECategory _currentCategory;

		Camera _mainCamera;
		Farmer _farmer;

		Dictionary<int, StorageGroup> _storageGroupDic;

		public GameObject prefProductButton;
		public GameObject prefDecorationButton;
		public GameObject prefStorageGroup;

		public GameObject plantingPanel;
		public GameObject decorationPanel;
		public GameObject storagePanel;

		public GameObject landTileFuncGroup;
		public GameObject objectTileFuncGroup;

		public GameObject productScrollViewContent;
		public GameObject decorationScrollViewContent;
		public GameObject storageScrollViewContent;

		public GameObject farmerObject;

		void Awake()
		{
			_storageGroupDic = new Dictionary<int, StorageGroup>();
			_currentCategory = ECategory.Default;

			_mainCamera = Camera.main;
			_farmer = farmerObject.GetComponent<Farmer>();
		}

		void Start()
		{
			CreatePlantScrollViewItem();
			CreateDecorationScrollViewItem();
		}

		void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject() && _currentCategory == ECategory.Default)
			{
				if (Input.GetMouseButton(0))
				{
					InputClick();
				}
			}
		}

		void InputClick()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, 10.0f))
			{
				if (hit.transform.tag == ObjectTag.FarmLand)
				{
					LandTile landTile = hit.transform.GetComponent<LandTile>();
					_farmer.SetTargetPosition(landTile);
				}
			}
		}

		#region Common Func
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
				case ECategory.Storage:
					ClosePanel(_currentCategory);
					break;
				default:
					break;
			}
		}

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
				case ECategory.Storage:
					_currentCategory = ECategory.Default;
					storagePanel.SetActive(false);
					break;
				default:
					break;
			}
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
			SceneManager.LoadScene(SceneName.MenuSceneName);
		}
		#endregion


		#region Object Tile Func
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

		public void ButtonEvent_RemoveObjectTile()
		{
			Point cursur = MapData.Instance.CurrentFarmerPoint;

			if (!ObjectTileManager.Instance.HasObjectTileAtPoint(cursur))
			{
				return;
			}

			PopupManager.PopupData data;
			data.text = "정말로 타일을 제거하시겠습니까?";
			data.okFlag = false;
			data.callBack = () =>
			{
				ObjectTileManager.Instance.RemoveObjectTile();
				objectTileFuncGroup.SetActive(false);
			};

			PopupManager.Instance.ShowPopup(data);
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
		#endregion


		#region Land Tile Func
		public void ButtonEvent_ChangeToGrassLand()
		{
			Point cursurPoint = MapData.Instance.CurrentFarmerPoint;

			LandTileManager.Instance.SetLandTileType(cursurPoint, LandTile.EType.Grass);
		}

		public void ButtonEvent_ChangeToCultivate()
		{
			Point cursurPoint = MapData.Instance.CurrentFarmerPoint;

			LandTileManager.Instance.SetLandTileType(cursurPoint, LandTile.EType.Cultivate);
		}
		#endregion


		#region Storage Func
		public void ButtonEvent_OpenStoragePanel()
		{
			_currentCategory = ECategory.Storage;

			storagePanel.SetActive(true);
			UpdateStorageGroupInfo();
		}

		void UpdateStorageGroupInfo()
		{
			var cropAmountDic = InventoryManager.Instance.CropAmountDic;

			foreach (var child in cropAmountDic)
			{
				if (_storageGroupDic.ContainsKey(child.Key))
				{
					_storageGroupDic[child.Key].SetAmount(child.Value);
				}
				else
				{
					GameObject obj = Instantiate(prefStorageGroup, storageScrollViewContent.transform);
					StorageGroup script = obj.transform.GetComponent<StorageGroup>();
					ProductData data = MapData.Instance.ProductDatas[child.Key];
					script.InitData(data);

					_storageGroupDic.Add(child.Key, script);
				}
			}
		}
		#endregion

	}

}
