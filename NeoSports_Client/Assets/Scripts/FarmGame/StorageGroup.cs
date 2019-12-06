using System.Collections;
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

		public void InitData(ProductData data)
		{
			Sprite sprite = ResourceManager.Instance.GetFarmAtlas(data.fullGrownSprite);
			cropImage.GetComponent<Image>().sprite = sprite;

			cropNameText.GetComponent<Text>().text = data.name;
			cropAmountText.GetComponent<Text>().text = "0 개";
		}

		public void SetAmount(int count)
		{
			cropAmountText.GetComponent<Text>().text = count.ToString() + "개";
		}

	}
}


