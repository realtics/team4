using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum eAIControlState
{
	Idle,
	Walk,
	Pull
}
public class AIPlayerController : PlayerController
{
	public override void InitController(Character controlCharacter, Player player)
	{
		base.InitController(controlCharacter,player);
		SetAIWithScene();
	}

	protected override void Update()//부모와 다르게 동작하기 위해서 빈 함수임에도 정의/
	{
		
	}

	void SetAIWithScene()
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
					StartCoroutine(nameof(ProcessPullRope));
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
	protected override void ProcessBasket()
	{
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

	protected override void ProcessJumpRope()
	{

	}

	protected new IEnumerator ProcessPullRope()
	{
		while (true)
		{
			if (RopePullGame.RopePullGameManager.Instance.SceneState != RopePullGame.RopePullGameManager.ESceneState.Play)
			{
				yield return new WaitForSeconds(1.0f);
			}
			else
			{
				if (_isSinglePlay)
				{
					_ownPlayer.PullRope();
				}
				else
				{
					_ownPlayer.NetworkPullRope();
				}
				yield return new WaitForSeconds(0.5f);
			}
		}
	}

	protected override void ProcessWaitRoom()
	{
		return;
	}

	#endregion
}
