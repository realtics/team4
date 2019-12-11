using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	const float AIShootRangeMinX = 2.0f;
	const float AIShootRangeMaxX = 15.0f;
	const float AIShootRangeMinY = -15.0f;
	const float AIShootRangeMaxY = 0.0f;
	enum ePlayerState
	{
		Move,
		Stop,
	};

	enum eLookDirection
	{
		Left = -1,
		Right = 1,
	};



	[HideInInspector]
	public BoxCollider2D _playerTrigger;
	[HideInInspector]
	public Camera mainCam;
	[HideInInspector]
	public bool isClickOn;
	[HideInInspector]
	public Vector2 targetPos;

	public GameObject directionArrowPrefab;
	public GameObject characterPrefab;
	public GameObject controllerPrefab;
	public BasketBallGame.BasketBall baksetballPrefab;


	GameObject _instChar;
	GameObject _instController;
	GameObject _instArrow;

	PlayerController _playerController;
	Character _character;

	float _powerSize;
	bool _isHost;
	SpirteOutlineshader _outlineshader;

	ePlayerState _state;
	eLookDirection _playerLookDirection;
	//To Do: 게임매니저로 옮겨서 플레이어로 이어주도록 해야함. 
	PoolFactory _ballFactory;

	void Start()
	{
		_ballFactory = new PoolFactory(baksetballPrefab);
		targetPos = transform.position;
		CachingValues();
		InitPlayer(_character, _playerController);
		SetPlayerDirection();
	}

	void Update()
	{
		if (_state == ePlayerState.Move)
		{
			if (_character != null)
			{
				if ((Vector2)transform.position == targetPos)
				{
					_character.EndRun();
					_state = ePlayerState.Stop;
					_outlineshader.StopWalkEffect();
					return;
				}
				MoveToTargetPos();
				_outlineshader.PlayWalkEffect();
			}
		}
	}

	void CachingValues()
	{
		_instChar = Instantiate(characterPrefab, this.transform);
		_instController = Instantiate(controllerPrefab, this.transform);
		_instArrow = Instantiate(directionArrowPrefab, this.transform);

		_character = _instChar.GetComponent<Character>();
		_playerController = _instController.GetComponent<PlayerController>();
		_playerTrigger = GetComponent<BoxCollider2D>();
		_outlineshader = _instChar.GetComponent<SpirteOutlineshader>();

		mainCam = Camera.main;
		_isHost = NetworkManager.Instance.isOwnHost;
	}

	#region public Player Function -Controller Use
	public void InitPlayer(Character character, PlayerController controller)
	{
		_character = character;
		_playerController = controller;
		_playerController.InitController(_character, this);
	}

	public void AimingShoot()
	{
		_instArrow.transform.position = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f);
		isClickOn = true;
	}

	public void CalculateShoot()
	{
		Vector2 target = mainCam.ScreenToWorldPoint(Input.mousePosition);
		float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

		float power = Vector2.Distance(target, transform.position);
		_powerSize = power * _character.status.strength;

		_instArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
		_instArrow.transform.localScale = new Vector3(_powerSize, _powerSize);
	}

	public void CalculateShootAuto()
	{
		Vector2 target = CalculateTargetAuto();

		float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

		_instArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);

		float power = Vector2.Distance(new Vector2(target.x, target.y), transform.position);

		_powerSize = power * _character.status.strength;
		_instArrow.transform.localScale = new Vector3(_powerSize, _powerSize);
	}

	public Vector2 CalculateTargetAuto()
	{
		Vector2 target = new Vector2
		{
			x = Random.Range(AIShootRangeMinX, AIShootRangeMaxX),
			y = Random.Range(AIShootRangeMinY, AIShootRangeMaxY)
		};

		return target;
	}


	public void ShootBall()
	{
		_instArrow.transform.localScale = new Vector3(0, 0, 0);
		isClickOn = false;

		Vector2 direction = _instArrow.transform.rotation * new Vector2(1, 0.0f) * _powerSize;
		_powerSize = 0.0f;

		BasketBallGame.BasketBall ball = _ballFactory.Get() as BasketBallGame.BasketBall;
		ball.ShotToTarget(direction);
		ball.Activate(transform.position, BasketBallGame.EBallOwner.LeftPlayer, "Ball");
		ball.destroyed += OnBallDestroyed;
	}

	public void ShootBallAuto()
	{
		isClickOn = false;

		Vector2 direction = _instArrow.transform.rotation * new Vector2(1, 0.0f) * _powerSize;
		_powerSize = 0.0f;

		BasketBallGame.BasketBall ball = _ballFactory.Get() as BasketBallGame.BasketBall;
		ball.ShotToTarget(direction);
		ball.Activate(transform.position, BasketBallGame.EBallOwner.LeftPlayer, "Ball");
		ball.destroyed += OnBallDestroyed;
	}

	void OnBallDestroyed(BasketBallGame.BasketBall usedBall)
	{//To Do: 게임매니저로 옮겨서 플레이어로 이어주도록 해야함. 
		usedBall.destroyed -= OnBallDestroyed;
		_ballFactory.Restore(usedBall);
	}

	void MoveToTargetPos()
	{
		if ((Vector2)transform.position != targetPos)
		{
			transform.position = Vector2.MoveTowards(transform.position, targetPos, _character.status.agility * Time.deltaTime);
		}
	}

	public void DecideTargetPos(Vector3 clickPos)
	{
		targetPos = (Vector2)mainCam.ScreenToWorldPoint(clickPos);

		_character.StartRun();
		_state = ePlayerState.Move;

		#region DecideDirection
		if (_character.transform.position.x < targetPos.x)
			_character.spriteRenderer.flipX = false;
		else
			_character.spriteRenderer.flipX = true;
		#endregion
	}

	public void SetFlipCharacter(bool isFlip)
	{
		_character.spriteRenderer.flipX = isFlip;
	}

	void SetPlayerDirection()
	{
		if (_character.spriteRenderer.flipX)
		{
			_playerLookDirection = eLookDirection.Left;
		}
		else if (!_character.spriteRenderer.flipX)
		{
			_playerLookDirection = eLookDirection.Right;
		}
	}

	public void PullRope()
	{
		RopePullGame.RopePullRope.Instance.PullRope((int)_playerLookDirection * _character.status.strength);
		_character.PullRopeAutoRelease();

		_outlineshader.PlayLineEffect();
	}

	public void NetworkPullRope()
	{
		if (_isHost)
			NetworkManager.Instance.SendRequestRopePull(_character.status.strength * -1);
		else
			NetworkManager.Instance.SendRequestRopePull(_character.status.strength);
	}

	#endregion

}
