using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public GameObject directionArrow;

	[HideInInspector]
	public BoxCollider2D _playerTrigger;
	[HideInInspector]
	public Camera _mainCam;
	[HideInInspector]
	public bool _isClickOn;

	public GameObject _characterPrefab;
	public PlayerController _playerController;
	float _powerSize;
	Character _character;

	//To Do: 게임매니저로 옮겨서 플레이어로 이어주도록 해야함. 
	public BasketBallGame.BasketBall baksetballPrefab;
	PoolFactory _ballFactory;

	private void Start()
	{
		_ballFactory = new PoolFactory(baksetballPrefab);
		CachingValues();
		InitPlayer(_character, _playerController);
	}
	void CachingValues()
	{
		_characterPrefab = Instantiate(_characterPrefab,this.transform);
		_character = _characterPrefab.GetComponent<Character>();
		_mainCam = Camera.main;
		_playerTrigger = GetComponent<BoxCollider2D>();
		_characterPrefab.SetActive(true);
	}

	#region public Player Function -Controller Use
	public void InitPlayer(Character character, PlayerController controller)
	{
		_character = character;
		//_character.
		_playerController = controller;
		_playerController.InitController(_character, this);
	}

	public void AimingShoot()
	{
		Debug.Log("AimShoot");
		directionArrow.transform.position = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f);
		_isClickOn = true;
	}

	public void CalculateShoot()
	{
		Vector2 target = _mainCam.ScreenToWorldPoint(Input.mousePosition);
		float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

		float power = Vector2.Distance(target, transform.position);
		_powerSize = power * _character.status.strength;

		directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
		directionArrow.transform.localScale = new Vector3(_powerSize , _powerSize);
	}

	public void ShootBall()
	{
		directionArrow.transform.localScale = new Vector3(0, 0, 0);
		_isClickOn = false;

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

	#endregion

}
