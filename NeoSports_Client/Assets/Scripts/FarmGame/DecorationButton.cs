using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace FarmGame
{
	public class DecorationButton : MonoBehaviour
	{
		public GameObject imageTile;
		public GameObject labelName;
		public GameObject labelRequireGold;

		DecorationData _data;

		public void SetData(DecorationData data)
		{
			_data = data;

			imageTile.GetComponent<Image>().sprite = ResourceManager.Instance.GetFarmAtlas(data.sprite);
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
			Point deployPoint = MapData.Instance.CurrentFarmerPoint;
			ObjectTileManager.Instance.DeployDecoration(deployPoint, _data.type);
		}
	}

}
