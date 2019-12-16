﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using FarmGame;


public class PlayerController : MonoBehaviour
{
	protected enum eControlScene
	{
		NotControl,
		PullRope,
		JumpRoPe,
		WaitRoom,
		BaksetBall,
		Farm
	}

	protected eControlScene _controlScene;
	protected Character _controlChar;
	protected Player _ownPlayer;
	protected bool _isSinglePlay;

	public virtual void InitController(Character controlCharacter, Player player)
	{
		_controlChar = controlCharacter;
		_ownPlayer = player;
		SetControlScene(SceneManager.GetActiveScene().name);
		if(NetworkManager.Instance !=null)
		_isSinglePlay = NetworkManager.Instance.IsSinglePlay();
	}

	protected virtual void Update() // To Do. Update 방식에서 이벤트 방식으로 추후 리팩토링
	{
		if (_controlChar == null)
			return;
		if (_controlScene == eControlScene.NotControl)
			return;
		ProcessInput();
	}

	void ProcessInput()
	{
		switch (_controlScene)
		{
			case eControlScene.BaksetBall:
				{
					ProcessBasket();
					break;
				}
			case eControlScene.JumpRoPe:
				{
					ProcessJumpRope();
					break;
				}
			case eControlScene.PullRope:
				{
					ProcessPullRope();
					break;
				}
			case eControlScene.WaitRoom:
				{
					ProcessWaitRoom();
					break;
				}
			case eControlScene.Farm:
				ProcessFarm();
				break;
			default:
				break;
		}
	}

	#region Input Case
	protected virtual void ProcessBasket()
	{
		if (BasketBallGame.BasketBallGameManager.Instance.GameState != BasketBallGame.BasketBallGameManager.EGameState.Playing)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (_ownPlayer._playerTrigger.OverlapPoint(_ownPlayer.mainCam.ScreenToWorldPoint(Input.mousePosition)))
			{
				_ownPlayer.AimingShoot();
			}
			else
			{
				_ownPlayer.DecideTargetPos(Input.mousePosition);
				return;
			}
		}
		else if (Input.GetMouseButton(0) && _ownPlayer.isClickOn)
		{
			_ownPlayer.CalculateShoot();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (_ownPlayer.isClickOn)
				_ownPlayer.ShootBall();
		}
	}

	protected virtual void ProcessJumpRope()
	{

	}

	protected void ProcessPullRope()
	{
		if (RopePullGame.RopePullGameManager.Instance.SceneState != RopePullGame.RopePullGameManager.ESceneState.Play)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonUp(0))
		{

			if (_isSinglePlay)
			{
				_ownPlayer.PullRope();
			}
			else
			{
				_ownPlayer.NetworkPullRope();
			}
		}
	}

	protected virtual void ProcessWaitRoom()
	{
		if (Input.GetMouseButtonDown(0))
			_ownPlayer.DecideTargetPos(Input.mousePosition);
		return;
	}

	void ProcessFarm()
	{
		if (!EventSystem.current.IsPointerOverGameObject() &&
				FarmUIManager.Instance.CurrentCategory == FarmUIManager.ECategory.Default)
		{
			if (Input.GetMouseButton(0))
			{
				FarmRayTarget();
			}
		}

		_ownPlayer.FarmUpdate();
	}

	void FarmRayTarget()
	{
		Ray ray = _ownPlayer.mainCam.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, 10.0f))
		{
			if (hit.transform.tag == ObjectTag.FarmLand)
			{
				LandTile landTile = hit.transform.GetComponent<LandTile>();
				_ownPlayer.SetTargetPosition(landTile);
			}
		}
	}
	#endregion

	#region SetSceneEnumFromString
	protected void SetControlScene(string sceneName)
	{
		if (sceneName.CompareTo(SceneName.MenuSceneName) == 0)
		{
			_controlScene = eControlScene.NotControl;
			return;
		}
		else if (sceneName.CompareTo(SceneName.NetworkBasketBallSceneName) == 0)
		{
			_controlScene = eControlScene.BaksetBall;
			return;
		}
		else if (sceneName.CompareTo(SceneName.BasketBallGameSceneName) == 0)
		{
			_controlScene = eControlScene.BaksetBall;
			return;
		}
		else if (sceneName.CompareTo(SceneName.NetworkRopeGameSceneName) == 0)
		{
			_controlScene = eControlScene.PullRope;
			return;
		}
		else if (sceneName.CompareTo(SceneName.RopeGameSceneName) == 0)
		{
			_controlScene = eControlScene.PullRope;
			return;
		}
		else if (sceneName.CompareTo(SceneName.JumpRopeGameSceneName) == 0)
		{
			_controlScene = eControlScene.JumpRoPe;
			return;
		}
		else if (sceneName.CompareTo(SceneName.WaitGameSceneName) == 0)
		{
			_controlScene = eControlScene.WaitRoom;
			return;
		}
		else if(sceneName.CompareTo(SceneName.FarmSceneName) == 0)
		{
			_controlScene = eControlScene.Farm;
			return;
		}
		_controlScene = eControlScene.NotControl;
		return;
	}
	#endregion
}
