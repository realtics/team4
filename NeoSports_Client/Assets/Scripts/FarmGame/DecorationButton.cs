#pragma warning disable CS0649
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace FarmGame
{
	public class DecorationButton : MonoBehaviour
	{
		[SerializeField]
		GameObject imageTile;
		[SerializeField]
		GameObject labelName;
		[SerializeField]
		GameObject labelRequireGold;
		[SerializeField]
		Button buttonScript;

		DecorationData _data;

		private void OnEnable()
		{
			if (ResourceManager.Instance.GetGoldResource() < _data.price)
			{
				buttonScript.interactable = false;
			}
			else
			{
				buttonScript.interactable = true;
			}
		}

		public void SetData(DecorationData data)
		{
			_data = data;

			imageTile.GetComponent<Image>().sprite = ResourceManager.Instance.GetFarmSprite(data.sprite);
			labelName.GetComponent<Text>().text = data.name;
			labelRequireGold.GetComponent<Text>().text = data.price.ToString();

			AddButtonEvent();
		}

		void AddButtonEvent()
		{
			Button button = GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => OnClickDeployDecoration());
		}

		void OnClickDeployDecoration()
		{
			if(ResourceManager.Instance.GetGoldResource() < _data.price)
			{
				return;
			}

			Point deployPoint = MapData.Instance.CurrentFarmerPoint;
			ObjectTileManager.Instance.DeployDecoration(deployPoint, _data.type);

			ResourceManager.Instance.AddGoldResource(-_data.price);
			FarmUIManager.Instance.UpdateGoldResourceLabel();
		}
	}

}
