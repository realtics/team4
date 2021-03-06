﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FarmGame
{

	public class StorageGroup : MonoBehaviour
	{
		public struct LoadData
		{
			public int productId;
			public int productCount;
		}

		public GameObject cropImage;
		public GameObject cropNameText;
		public GameObject cropAmountText;

		public void InitData(ProductData data, int count)
		{
			Sprite sprite = ResourceManager.Instance.GetFarmSprite(data.fullGrownSprite);
			cropImage.GetComponent<Image>().sprite = sprite;

			cropNameText.GetComponent<Text>().text = data.name;
			cropAmountText.GetComponent<Text>().text = count.ToString("N0") + "개";
		}

		public void SetAmount(int count)
		{
			cropAmountText.GetComponent<Text>().text = count.ToString("N0") + "개";
		}

	}
}


