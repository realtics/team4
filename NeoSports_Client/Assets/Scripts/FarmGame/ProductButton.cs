using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace FarmGame
{
	public class ProductButton : MonoBehaviour
	{
		[SerializeField]
		GameObject imageLessGrown;
		[SerializeField]
		GameObject imageFullGrown;
		[SerializeField]
		GameObject labelName;
		[SerializeField]
		GameObject labelGrownTime;
		[SerializeField]
		GameObject labelRequireGold;
		[SerializeField]
		Button buttonScript;

		ProductData _data;

		private void OnEnable()
		{
			int goldAmount = ResourceManager.Instance.GetGoldResource();
			if(goldAmount < _data.price)
			{
				buttonScript.interactable = false;
			}
			else
			{
				buttonScript.interactable = true;
			}
		}

		public void SetData(ProductData data)
		{
			_data = data;

			imageLessGrown.GetComponent<Image>().sprite = ResourceManager.Instance.GetFarmSprite(data.lessGrownSprite);
			imageFullGrown.GetComponent<Image>().sprite = ResourceManager.Instance.GetFarmSprite(data.fullGrownSprite);

			labelName.GetComponent<Text>().text = data.name;

			labelGrownTime.GetComponent<Text>().text = data.grownTime.ToString() + "분";
			labelRequireGold.GetComponent<Text>().text = data.price.ToString();

			AddButtonEvent();
		}

		void AddButtonEvent()
		{
			Button button = GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => OnClickPlantProduct());
		}

		void OnClickPlantProduct()
		{
			if(ResourceManager.Instance.GetGoldResource() < _data.price)
			{
				return;
			}

			Point plantPoint = MapData.Instance.CurrentFarmerPoint;
			ObjectTileManager.Instance.PlantProduct(plantPoint, _data.type);

			ResourceManager.Instance.AddGoldResource(-_data.price);
			FarmUIManager.Instance.UpdateGoldResourceLabel();
		}
	}

}
