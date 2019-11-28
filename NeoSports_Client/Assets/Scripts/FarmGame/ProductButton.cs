using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class ProductButton : MonoBehaviour
{

	public SpriteAtlas farmLandTileAtlas;
	public GameObject imageLessGrown;
	public GameObject imageFullGrown;
	public GameObject labelName;
	public GameObject labelGrownTime;
	public GameObject labelRequireGold;

	ProductData _data;

	public void SetData(ProductData data)
	{
		_data = data;

		imageLessGrown.GetComponent<Image>().sprite = farmLandTileAtlas.GetSprite(data.lessGrownSprite);
		imageFullGrown.GetComponent<Image>().sprite = farmLandTileAtlas.GetSprite(data.fullGrownSprite);

		labelName.GetComponent<Text>().text = data.name;

		int grownTime = (data.grownHour * 60) + data.grownMin;
		labelGrownTime.GetComponent<Text>().text = grownTime.ToString() + "분";
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
		Point plantPoint = MapData.Instance.CurrentFarmerPoint;
		ObjectTileManager.Instance.PlantProduct(plantPoint, _data.type);
	}

}
