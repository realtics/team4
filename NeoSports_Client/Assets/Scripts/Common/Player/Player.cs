using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject directionArrow;

	[HideInInspector]
	public BoxCollider2D _playerTrigger;
	[HideInInspector]
	public Camera mainCam;
	[HideInInspector]
	public bool isClickOn;
	[HideInInspector]
	public Vector2 targetPos;

	public GameObject _characterPrefab;
	public GameObject _controllerPrefab;
	public BasketBallGame.BasketBall baksetballPrefab;


	GameObject _instChar;
	GameObject _instController;

	PlayerController _playerController;
	Character _character;
	float _powerSize;
	//To Do: 게임매니저로 옮겨서 플레이어로 이어주도록 해야함. 

	PoolFactory _ballFactory;

	void Start()
	{
		_ballFactory = new PoolFactory(baksetballPrefab);
		targetPos = transform.position;
		CachingValues();
		InitPlayer(_character, _playerController);
	}

	void Update()
	{
		MoveToTargetPos();
	}

	void CachingValues()
	{
		_instChar =Instantiate(_characterPrefab,this.transform);
		_instController = Instantiate(_controllerPrefab, this.transform);
		_character = _instChar.GetComponent<Character>();
		_playerController = _instController.GetComponent<PlayerController>();

		mainCam = Camera.main;
		_playerTrigger = GetComponent<BoxCollider2D>();
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
		directionArrow.transform.position = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f);
		isClickOn = true;
	}

	public void CalculateShoot()
	{
		Vector2 target = mainCam.ScreenToWorldPoint(Input.mousePosition);
		float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

		float power = Vector2.Distance(target, transform.position);
		_powerSize = power * _character.status.strength;

		directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
		directionArrow.transform.localScale = new Vector3(_powerSize , _powerSize);
	}

	public void ShootBall()
	{
		directionArrow.transform.localScale = new Vector3(0, 0, 0);
		isClickOn = false;

		Vector2 direction = directionArrow.transform.rotation * new Vector2(1, 0.0f) * _powerSize;
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
		else
		{
			_character.EndRun();
		}
	}

	public void DecideTargetPos(Vector3 clickPos)
	{
		targetPos = (Vector2)mainCam.ScreenToWorldPoint(clickPos);

		_character.StartRun();

		#region DecideDirection
		if (_character.transform.position.x < targetPos.x)
			_character.spriteRenderer.flipX = false;
		else
			_character.spriteRenderer.flipX = true;
		#endregion
	}

	#endregion

}
