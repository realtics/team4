using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FarmGame
{
	public class Farmer : MonoBehaviour
	{

		public enum State
		{
			Idle,
			Move,
			Acting
		}

		const float MoveSpeed = 3.0f;

		Vector3 _targetPosition;
		State _currentState;

		Camera _mainCamera;
		LandTile _currentLandTile;
		ProductTile _currentProductTile;

		private void Awake()
		{
			_mainCamera = Camera.main;

			_currentState = State.Idle;
		}

		private void Start()
		{
			Point startPoint = new Point(2, 2);
			MapData.Instance.CurrentFarmerPoint = startPoint;
			FarmUIManager.Instance.harvestButtonPressed += HarvestCurrentProduct;
		}

		private void Update()
		{
			if (!EventSystem.current.IsPointerOverGameObject() &&
				FarmUIManager.Instance.CurrentCategory == FarmUIManager.ECategory.Default)
			{
				if (Input.GetMouseButton(0))
				{
					InputClick();
				}
			}

			// Update
			switch (_currentState)
			{
				case State.Idle:
					break;
				case State.Move:
					MoveToTargetPosition();
					CheckStopMove();
					break;
				case State.Acting:
					break;
				default:
					break;
			}

			SyncCameraPosition();
		}

		void InputClick()
		{
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, 10.0f))
			{
				if (hit.transform.tag == ObjectTag.FarmLand)
				{
					LandTile landTile = hit.transform.GetComponent<LandTile>();
					SetTargetPosition(landTile);
				}
			}
		}

		public void SetTargetPosition(LandTile landTile)
		{
			LeaveTile();
			_targetPosition = landTile.transform.position;
			_targetPosition.z = transform.localPosition.z;
			_currentLandTile = landTile;
			_currentState = State.Move;
		}

		void MoveToTargetPosition()
		{
			transform.position = Vector3.MoveTowards(transform.position, _targetPosition, MoveSpeed * Time.deltaTime);
		}

		void CheckStopMove()
		{
			Vector3 currentPosition = transform.position;

			if (currentPosition == _targetPosition)
			{
				_currentState = State.Idle;
				EnterTile();
				return;
			}
		}

		void SyncCameraPosition()
		{
			Vector3 cameraPosition = _mainCamera.transform.position;

			cameraPosition.x = transform.position.x;
			cameraPosition.y = transform.position.y;

			_mainCamera.transform.position = cameraPosition;
		}

		void EnterTile()
		{
			Point currentPoint = _currentLandTile.MapPoint;
			MapData.Instance.CurrentFarmerPoint = currentPoint;
			_currentLandTile.Highlight = true;

			_currentProductTile = ObjectTileManager.Instance.GetProductTileAtPoint(currentPoint);

			if(_currentProductTile != null)
			{
				// 수확 기능
				if (_currentProductTile.CanHarvest)
				{
					FarmUIManager.Instance.HarvestButtonActive = true;
				}
				else
				{
					StartCoroutine(CheckCanHarvestEverySeconds());
				}

				// 상단 
				FarmUIManager.Instance.ProductInfoGroupActive = true;

				float grownSpeed = 0.0f;
				switch (_currentLandTile.Type)
				{
					case LandTile.BadlandType:
						grownSpeed = -0.5f;
						break;
					case LandTile.GrassType:
						grownSpeed = 0.0f;
						break;
					case LandTile.CultivateType:
						grownSpeed = 0.5f;
						break;
					default:
						Debug.LogWarning("Unknown Land Type!");
						break;
				}

				FarmUIManager.Instance.SetProductInfoData(_currentProductTile.ProductData, grownSpeed);
			}

		}

		void LeaveTile()
		{
			if (_currentLandTile != null)
			{
				_currentLandTile.Highlight = false;
				FarmUIManager.Instance.HarvestButtonActive = false;
				FarmUIManager.Instance.ProductInfoGroupActive = false;
				StopCoroutine(CheckCanHarvestEverySeconds());
			}
		}

		IEnumerator CheckCanHarvestEverySeconds()
		{
			while (true)
			{
				if(_currentProductTile == null)
				{
					yield break;
				}

				if (_currentProductTile.CanHarvest)
				{
					FarmUIManager.Instance.HarvestButtonActive = true;
					yield break;
				}
				else
				{
					yield return new WaitForSeconds(1.0f);
				}
			}
		}

		void HarvestCurrentProduct()
		{
			if(_currentProductTile != null)
			{
				if (_currentProductTile.CanHarvest)
				{
					_currentProductTile.HarvestProduct();
				}
			}
		}
	}
}
