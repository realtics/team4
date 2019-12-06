using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum eControlScene
{
	NotControl,
	PullRope,
	JumpRoPe,
	WaitRoom,
	BaksetBall,
}
public class PlayerController : MonoBehaviour
{
	Character _controlChar;
	eControlScene _controlScene;
	Player _ownPlayer;

	public void InitController(Character controlCharacter, Player player)
	{
		_controlChar = controlCharacter;
		_ownPlayer = player;
		SetControlScene(SceneManager.GetActiveScene().name);	
	}

	void Update() // To Do. Update 방식에서 이벤트 방식으로 추후 리팩토링
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
			default:
				break;
		}
	}

	#region Input Case
	void ProcessBasket()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (_ownPlayer._playerTrigger.OverlapPoint(_ownPlayer._mainCam.ScreenToWorldPoint(Input.mousePosition)))
			{
				_ownPlayer.AimingShoot();
			}
		}
		else if (Input.GetMouseButton(0) && _ownPlayer._isClickOn)
		{
			_ownPlayer.CalculateShoot();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			_ownPlayer.ShootBall();
		}	
	}

	void ProcessJumpRope()
	{
		
    }

	void ProcessPullRope()
	{ 

	}

	void ProcessWaitRoom()
	{
		
	}

	#endregion

	#region SetSceneEnumFromString
	void SetControlScene(string sceneName)
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
		_controlScene = eControlScene.NotControl;
		return;
	}
	#endregion
}
