﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

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

	Vector2	_targetPosition;
	State	_currentState;

	Camera _mainCamera;
	LandTile _currentLandTile;

	void Awake()
	{
		_mainCamera = Camera.main;

		_currentState = State.Idle;
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

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.tag == FarmObstacleTag)
		{
			_currentState = State.Idle;
			EnterTile();
		}
	}

	public void SetTargetPosition(LandTile landTile)
	{
		LeaveTile();
		_targetPosition = landTile.transform.position;
		_currentLandTile = landTile;
		_currentState = State.Move;
	}

	void MoveToTargetPosition()
	{
		transform.position = Vector2.MoveTowards(transform.position, _targetPosition, MoveSpeed * Time.deltaTime);
	}

	void CheckStopMove()
	{
		Vector2 currentPosition = transform.position;

		if(currentPosition == _targetPosition)
		{
			Debug.Log("Target Position Located!");
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
	}

	void LeaveTile()
	{
		if(_currentLandTile != null)
		{
			_currentLandTile.Highlight = false;
		}
	}
}