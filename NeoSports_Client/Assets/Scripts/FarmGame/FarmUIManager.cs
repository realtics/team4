using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmUIManager : MonoBehaviour
{

	public enum Category
	{
		Reclaim,
		HoeField,
		Planting,
		Decoration,
		Cleaning,
		Eliminate
	}

	const float DoubleTouchDuration = 0.5f;

	int _clickCount;
	float _touchDuration;
	Touch _touch;

	Camera _mainCamera;
	Farmer _farmer;

	public GameObject farmerObject;

	void Awake()
	{
		_mainCamera = Camera.main;
		_farmer = farmerObject.GetComponent<Farmer>();
	}

	void Start()
	{
		
	}

	void Update()
	{
		InputClick();
		//InputTouch();
	}

	#region Double Click
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

			print("ray.origin: " + ray.origin);
			Debug.Log("Double Click!");
			Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.blue, 1.0f);


			if(Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				Debug.Log("Hit");
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

	#region Touch Event
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
		if(hitObject.tag == ObjectTag.FarmLand)
		{
			LandTile landTile = hitObject.GetComponent<LandTile>();
			_farmer.SetTargetPosition(landTile);
		}
	}

	public void ShowCategory(Category category)
	{

	}

}
