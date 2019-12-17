using System.Collections;
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
		JumpRope,
		WaitRoom,
		BasketBall,
		Farm,
		FriendFarm
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
			case eControlScene.BasketBall:
				{
					ProcessBasket();
					break;
				}
			case eControlScene.JumpRope:
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
			case eControlScene.FriendFarm:
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
		switch (sceneName)
		{
			case SceneName.MenuSceneName:
				_controlScene = eControlScene.NotControl;
				break;
			case SceneName.NetworkBasketBallSceneName:
				_controlScene = eControlScene.BasketBall;
				break;
			case SceneName.BasketBallGameSceneName:
				_controlScene = eControlScene.BasketBall;
				break;
			case SceneName.NetworkRopeGameSceneName:
				_controlScene = eControlScene.PullRope;
				break;
			case SceneName.RopeGameSceneName:
				_controlScene = eControlScene.PullRope;
				break;
			case SceneName.JumpRopeGameSceneName:
				_controlScene = eControlScene.JumpRope;
				break;
			case SceneName.WaitGameSceneName:
				_controlScene = eControlScene.WaitRoom;
				break;
			case SceneName.FarmSceneName:
				_controlScene = eControlScene.Farm;
				break;
			case SceneName.FriendFarmSceneName:
				_controlScene = eControlScene.FriendFarm;
				break;
			default:
				_controlScene = eControlScene.NotControl;
				break;
		}
		
	}
	#endregion
}
