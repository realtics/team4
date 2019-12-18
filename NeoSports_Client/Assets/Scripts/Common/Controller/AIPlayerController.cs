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
	public override void InitController(Character controlCharacter, Player player, bool isControl = true)
	{
		base.InitController(controlCharacter,player, isControl);
		SetAIWithScene();
	}

	protected override void Update()//부모와 다르게 동작하기 위해서 빈 함수임에도 정의/
	{
		
	}

	void SetAIWithScene()
	{
		switch (_controlScene)
		{
			case eControlScene.BasketBall:
				{
                    StartCoroutine(nameof(ProcessBasket));
                    break;
				}
			case eControlScene.JumpRope:
				{
                    //StartCoroutine(nameof(JumpRoPe));
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
	protected new IEnumerator ProcessBasket()
	{
        while (true)
        {
            if (BasketBallGame.BasketBallGameManager.Instance.GameState == BasketBallGame.BasketBallGameManager.EGameState.Playing)
            {
				_ownPlayer.AimingShoot();
				_ownPlayer.CalculateShootAuto();		
                _ownPlayer.ShootBallAuto();
                
            }
            yield return new WaitForSeconds(1.0f);
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
