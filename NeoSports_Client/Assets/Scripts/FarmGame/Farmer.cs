using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

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
		const string FarmObstacleTag = "FarmObstacle";

		Vector3 _targetPosition;
		State _currentState;

		Camera _mainCamera;
		LandTile _currentLandTile;

		void Awake()
		{
			_mainCamera = Camera.main;

			_currentState = State.Idle;
		}

		void Start()
		{
			Point startPoint = new Point(2, 2);
			MapData.Instance.CurrentFarmerPoint = startPoint;
		}

		void Update()
		{
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
			_currentLandTile.Highlight = true;
			MapData.Instance.CurrentFarmerPoint = _currentLandTile.MapPoint;
		}

		void LeaveTile()
		{
			if (_currentLandTile != null)
			{
				_currentLandTile.Highlight = false;
			}
		}
	}
}
