﻿#pragma warning disable CS0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
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

		#region Inspector Linking
		// Prefab
		[SerializeField]
		GameObject prefProductButton;
		[SerializeField]
		GameObject prefDecorationButton;
		[SerializeField]
		GameObject prefStorageGroup;

		// Panel
		[SerializeField]
		GameObject plantingPanel;
		[SerializeField]
		GameObject decorationPanel;
		[SerializeField]
		GameObject storagePanel;

		// Func Group
		[SerializeField]
		GameObject landTileFuncGroup;
		[SerializeField]
		GameObject objectTileFuncGroup;
		[SerializeField]
		Text changeToGrassLandPriceText;
		[SerializeField]
		Text changeToCultivateLandPriceText;
		[SerializeField]
		Button changeToGrassLandButton;
		[SerializeField]
		Button changeToCultivateLandButton;
		[SerializeField]
		GameObject harvestButton;

		// Garbage Tile Func Group
		[SerializeField]
		GameObject cleaningGarbageGroup;
		[SerializeField]
		Button cleaningGarbageButton;
		[SerializeField]
		Text cleaningGarbagePrice;
		
		// Object Tile Func Group
		[SerializeField]
		Button plantProductButton;
		[SerializeField]
		Button deployDecorationButton;
		[SerializeField]
		Button removeTileButton;
		[SerializeField]
		Text productPlantEffectText;

		// Scroll View
		[SerializeField]
		GameObject productScrollViewContent;
		[SerializeField]
		GameObject decorationScrollViewContent;
		[SerializeField]
		GameObject storageScrollViewContent;

		// Product Info
		[SerializeField]
		GameObject productInfoGroup;
		[SerializeField]
		Image productInfoLessGrownImage;
		[SerializeField]
		Image productInfoFullGrownImage;
		[SerializeField]
		Text productInfoNameText;
		[SerializeField]
		Text productInfoEffectText;

		// Resource Info
		[SerializeField]
		Text goldResourceAmountText;
		#endregion

		public Action harvestButtonPressed;
		public Action cleaningGarbageButtonPressed;

		Dictionary<int, StorageGroup> _storageGroupDic;

		#region Property
		public ECategory CurrentCategory { get; private set; }
		public bool HarvestButtonActive {
			set { harvestButton.SetActive(value); }
		}
		public bool ProductInfoGroupActive {
			set { productInfoGroup.SetActive(value); }
		}
		#endregion

		private void Awake()
		{
			_storageGroupDic = new Dictionary<int, StorageGroup>();
			CurrentCategory = ECategory.Default;
			if (FriendFarmManager.Instance == null)
			{
				InitLandPriceLabel();
			}

		}

		private void Start()
		{
			if (FriendFarmManager.Instance == null)
			{
				CreatePlantScrollViewItem();
				CreateDecorationScrollViewItem();
				UpdateGoldResourceLabel();
			}
		}

		void InitLandPriceLabel()
		{
			changeToGrassLandPriceText.text = LandTile.ChangeToGrassPrice.ToString("N0") + " 사용";
			changeToCultivateLandPriceText.text = LandTile.ChangeToCultivatePrice.ToString("N0") + " 사용";
		}

		#region Common Func
		public void EventBackButton()
		{
			switch (CurrentCategory)
			{
				case ECategory.Default:
					ShowExitToMainMenuPopup();
					break;
				case ECategory.Plant:
					ClosePanel(CurrentCategory);
					break;
				case ECategory.Decoration:
					ClosePanel(CurrentCategory);
					break;
				case ECategory.Storage:
					ClosePanel(CurrentCategory);
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
					CurrentCategory = ECategory.Default;
					plantingPanel.SetActive(false);
					break;
				case ECategory.Decoration:
					CurrentCategory = ECategory.Default;
					decorationPanel.SetActive(false);
					break;
				case ECategory.Storage:
					CurrentCategory = ECategory.Default;
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
			if(FriendFarmManager.Instance != null)
			{
				FriendFarmManager.Instance.DestroySelf();
			}
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
			CurrentCategory = ECategory.Plant;

			objectTileFuncGroup.SetActive(false);
			plantingPanel.SetActive(true);
		}

		public void EventOpenDecorationPanelButton()
		{
			CurrentCategory = ECategory.Decoration;

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
			var productDatas = MapData.Instance.ProductDataDic;

			foreach (var item in productDatas)
			{
				GameObject obj = Instantiate(prefProductButton, productScrollViewContent.transform);
				obj.GetComponent<ProductButton>().SetData(item.Value);
			}
		}

		void CreateDecorationScrollViewItem()
		{
			var decorationDatas = MapData.Instance.DecorationDataDic;

			foreach (var item in decorationDatas)
			{
				GameObject obj = Instantiate(prefDecorationButton, decorationScrollViewContent.transform);
				obj.GetComponent<DecorationButton>().SetData(item.Value);
			}
		}
		public void ObjectTileFuncButtonInteract(bool canDeployTile)
		{
			plantProductButton.interactable = canDeployTile;
			deployDecorationButton.interactable = canDeployTile;
			removeTileButton.interactable = !canDeployTile;
			productPlantEffectText.gameObject.SetActive(canDeployTile);

			cleaningGarbageGroup.SetActive(false);
		}

		public void GarbageTileFuncButtonInteract(GarbageData data)
		{
			removeTileButton.interactable = false;

			cleaningGarbagePrice.text = data.removeCost.ToString("N0") + "사용";
			if (data.removeCost <= ResourceManager.Instance.GetGoldResource())
			{
				cleaningGarbageButton.interactable = true;
			}
			else
			{
				cleaningGarbageButton.interactable = false;
			}

			cleaningGarbageGroup.SetActive(true);
		}

		public void GarbageTileFuncGroupInactive()
		{
			cleaningGarbageGroup.SetActive(false);
		}
		#endregion


		#region Land Tile Func
		public void ButtonEvent_ChangeToGrassLand()
		{
			ResourceManager.Instance.AddGoldResource(-LandTile.ChangeToGrassPrice);
			UpdateLandTileChangeInteract(LandTile.GrassType);
			UpdateGoldResourceLabel();

			Point cursurPoint = MapData.Instance.CurrentFarmerPoint;

			LandTileManager.Instance.SetLandTileType(cursurPoint, LandTile.GrassType);
			ObjectTileManager.Instance.UpdateHarvestTimeByLandChange(cursurPoint, LandTile.GrassType);
			UpdatePlantProductEffectText(LandTile.GrassGrownSpeedScale);
		}

		public void ButtonEvent_ChangeToCultivate()
		{
			ResourceManager.Instance.AddGoldResource(-LandTile.ChangeToCultivatePrice);
			UpdateLandTileChangeInteract(LandTile.CultivateType);
			UpdateGoldResourceLabel();

			Point cursurPoint = MapData.Instance.CurrentFarmerPoint;

			LandTileManager.Instance.SetLandTileType(cursurPoint, LandTile.CultivateType);
			ObjectTileManager.Instance.UpdateHarvestTimeByLandChange(cursurPoint, LandTile.CultivateType);
			UpdatePlantProductEffectText(LandTile.CultivateGrownSpeedScale);
		}

		public void UpdatePlantProductEffectText(float grownScale)
		{
			grownScale -= 1.0f;
			grownScale = -grownScale;
			productPlantEffectText.text = "성장 속도 " + (grownScale * 100).ToString("+0;-#") + " % ";

			if (grownScale < 0.0f)
			{
				productPlantEffectText.color = UnityEngine.Color.red;
			}
			else if (grownScale > 0.0f)
			{
				productPlantEffectText.color = UnityEngine.Color.green;
			}
			else
			{
				productPlantEffectText.color = new UnityEngine.Color(0.7f, 0.7f, 0.7f);
			}
		}

		public void UpdateLandTileChangeInteract(string type)
		{
			switch (type)
			{
				case LandTile.BadlandType:
					changeToGrassLandButton.interactable = true;
					changeToCultivateLandButton.interactable = true;
					break;
				case LandTile.GrassType:
					changeToGrassLandButton.interactable = false;
					changeToCultivateLandButton.interactable = true;
					break;
				case LandTile.CultivateType:
					changeToGrassLandButton.interactable = true;
					changeToCultivateLandButton.interactable = false;
					break;
				default:
					changeToGrassLandButton.interactable = true;
					changeToCultivateLandButton.interactable = true;
					break;
			}

			int goldAmount = ResourceManager.Instance.GetGoldResource();

			if (changeToGrassLandButton.interactable)
			{
				changeToGrassLandButton.interactable = goldAmount < LandTile.ChangeToGrassPrice ? false : true;
			}

			if (changeToCultivateLandButton.interactable)
			{
				changeToCultivateLandButton.interactable = goldAmount < LandTile.ChangeToCultivatePrice ? false : true;
			}
		}
		#endregion


		#region Storage Func
		public void ButtonEvent_OpenStoragePanel()
		{
			CurrentCategory = ECategory.Storage;

			UpdateStorageGroupInfo();
			storagePanel.SetActive(true);
		}

		void UpdateStorageGroupInfo()
		{
			var cropAmountDic = ResourceManager.Instance.ProductResourceDic;

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
					ProductData data = MapData.Instance.ProductDataDic[child.Key];
					script.InitData(data, child.Value);

					_storageGroupDic.Add(child.Key, script);
				}
			}
		}
		#endregion


		#region Product Info
		public void SetProductInfoData(ProductData data, float grownScale)
		{
			grownScale -= 1.0f;
			grownScale = -grownScale;
			productInfoLessGrownImage.sprite = ResourceManager.Instance.GetFarmSprite(data.lessGrownSprite);
			productInfoFullGrownImage.sprite = ResourceManager.Instance.GetFarmSprite(data.fullGrownSprite);

			productInfoNameText.text = data.name;

			productInfoEffectText.text = "성장 속도 " + (grownScale * 100).ToString("+0;-#") + " % ";
			if(grownScale < 0.0f)
			{
				productInfoEffectText.color = UnityEngine.Color.red;
			}
			else if(grownScale > 0.0f)
			{
				productInfoEffectText.color = UnityEngine.Color.green;
			}
			else
			{
				productInfoEffectText.color = new UnityEngine.Color(0.3f, 0.3f, 0.3f);
			}
		}
		#endregion

		public void UpdateGoldResourceLabel()
		{
			int value = ResourceManager.Instance.GetGoldResource();
			goldResourceAmountText.text = value.ToString("N0");		// "N0" to 00,000 (for comma)
		}

		public void ButtonEvent_HarvestProduct()
		{
			harvestButton.SetActive(false);
			harvestButtonPressed();
		}

		public void ButtonEvent_CleaningGarbage()
		{
			cleaningGarbageGroup.SetActive(false);
			cleaningGarbageButtonPressed();
		}

		public void ButtonEvent_DEBUG_GoldIncrease()
		{
			ResourceManager.Instance.AddGoldResource(100);
			UpdateGoldResourceLabel();
		}

	}

}
