using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using FarmGame;

public class Player : MonoBehaviour
{
	const float AIShootRangeMinX = 2.0f;
	const float AIShootRangeMaxX = 15.0f;
	const float AIShootRangeMinY = -15.0f;
	const float AIShootRangeMaxY = 0.0f;
	const int PowerLimit = 3;
	const int PowerOffset = 5;

	public enum eLookDirection
	{
		Left = -1,
		Right = 1,
	};

	public enum ePlayerState
	{
		Move,
		Stop,
		Farm
	};

	enum eFarmState
	{
		Idle,
		Move,
		Acting
	}

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
	public GameObject equipmentPrefab;
	public BasketBallGame.BasketBall baksetballPrefab;

	public ePlayerState MoveState { get; private set; }

	GameObject _instChar;
	GameObject _instController;
	GameObject _instArrow;
	GameObject _instEquipment;

	PlayerController _playerController;
	PlayerEquipment _playerEquipment;

	float _powerSize;
	bool _isHost;
	bool _isControlPlayer;
	SpirteOutlineshader _outlineshader;

	eFarmState _farmState;
	eLookDirection _playerLookDirection;
	//To Do: 게임매니저로 옮겨서 플레이어로 이어주도록 해야함. 
	PoolFactory _ballFactory;

	#region Property
	public Character OwnCharacter { get; set; }
	#endregion

	private void Awake()
	{
		_ballFactory = new PoolFactory(baksetballPrefab);
		targetPos = transform.position;
		mainCam = Camera.main;
	}

	public void Initialize(bool isControlPlayer = true)
	{
		_isControlPlayer = isControlPlayer;
		MoveState = ePlayerState.Stop;
		CachingValues();
		InitPlayerDirection();
		InitPlayer(OwnCharacter, _playerController);
		if (NetworkManager.Instance != null)
			_isHost = NetworkManager.Instance.isOwnHost;
	}

	//void Start()
	//{
	//	CachingValues();
	//	InitPlayerDirection();
	//	InitPlayer(OwnCharacter, _playerController);
	//	if (NetworkManager.Instance != null)
	//		_isHost = NetworkManager.Instance.isOwnHost;
	//}

	void Update()
	{
		// ePlayerState의 Stop을 사용하면 안됨, Farm Scene에서 의도하지 않은 일이 벌어질 수 있음

		if (MoveState == ePlayerState.Move)
		{
			if (OwnCharacter != null)
			{
				if ((Vector2)transform.position == targetPos)
				{
					OwnCharacter.EndRun();
					MoveState = ePlayerState.Stop;
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
		_instEquipment = Instantiate(equipmentPrefab, this.transform);

		OwnCharacter = _instChar.GetComponent<Character>();
		_playerController = _instController.GetComponent<PlayerController>();
		_playerTrigger = GetComponent<BoxCollider2D>();
		_outlineshader = _instChar.GetComponent<SpirteOutlineshader>();

		_playerEquipment = _instEquipment.GetComponent<PlayerEquipment>();
	}

	#region public Player Function -Controller Use
	public void InitPlayer(Character character, PlayerController controller)
	{
		OwnCharacter = character;
		_playerController = controller;
		_playerController.InitController(OwnCharacter, this, _isControlPlayer);
		_playerEquipment.InitializeEquipItem(this);
	}

	public void AimingShoot()
	{
		_instArrow.transform.position = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f);
		isClickOn = true;
	}

	public void CalculateShoot()
	{
		if (MoveState == ePlayerState.Move)
			return;

		Vector2 target = mainCam.ScreenToWorldPoint(Input.mousePosition);
		float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

		float power = Vector2.Distance(target, transform.position);

		if (power >= PowerLimit)
			power = PowerLimit;
		_powerSize = power * OwnCharacter.status.strength * PowerOffset;

		_instArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
		_instArrow.transform.localScale = new Vector3(_powerSize, _powerSize);

	}

	public void CalculateShootAuto()
	{
		Vector2 target = CalculateTargetAuto();

		float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

		_instArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);

		float power = Vector2.Distance(new Vector2(target.x, target.y), transform.position);

		_powerSize = power * OwnCharacter.status.strength;
		//_instArrow.transform.localScale = new Vector3(_powerSize, _powerSize);
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
		if (MoveState == ePlayerState.Move)
			return;

		_instArrow.transform.localScale = new Vector3(0, 0, 0);
		isClickOn = false;

		Vector2 direction = _instArrow.transform.rotation * new Vector2(1, 0.0f) * _powerSize;



		BasketBallGame.BasketBall ball = _ballFactory.Get() as BasketBallGame.BasketBall;
		ball.ShotToTarget(direction);
		ball.Activate(transform.position, BasketBallGame.EBallOwner.LeftPlayer, "Ball");
		ball.destroyed += OnBallDestroyed;

		if (!NetworkManager.Instance.IsSinglePlay())
		{
			NetworkManager.Instance.SendRequestFireBall(_powerSize, direction.x, direction.y);
		}

		_powerSize = 0.0f;
	}

	public void ShootBall(float power, float angleX, float angleY)
	{
		if (MoveState == ePlayerState.Move)
			return;
		//Vector2 direction = _instArrow.transform.rotation * new Vector2(1, angleY) * power;
		Vector2 direction;
		direction.x = angleX;
		direction.y = angleY;

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
		ball.Activate(transform.position, BasketBallGame.EBallOwner.AI, "AIBall");
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
			transform.position = Vector2.MoveTowards(transform.position, targetPos, OwnCharacter.status.agility * Time.deltaTime);

		}
	}

	public void DecideTargetPos(Vector3 clickPos)
	{
		targetPos = (Vector2)mainCam.ScreenToWorldPoint(clickPos);
		if (!NetworkManager.Instance.IsSinglePlay())
		{
			NetworkManager.Instance.SendRequestMove(targetPos.x, targetPos.y, 0);
		}

		OwnCharacter.StartRun();
		MoveState = ePlayerState.Move;

		#region DecideDirection
		if (OwnCharacter.transform.position.x < targetPos.x)
			SetFlipCharacter(false);
		else
			SetFlipCharacter(true);
		#endregion
	}

	public void NetworkDecideTargetPos(Vector2 NetworktargetPos)
	{
		targetPos = NetworktargetPos;
		OwnCharacter.StartRun();
		MoveState = ePlayerState.Move;

		#region DecideDirection
		if (OwnCharacter.transform.position.x < targetPos.x)
			SetFlipCharacter(false);
		else
			SetFlipCharacter(true);
		#endregion
	}

	public void SetFlipCharacter(bool isFlip)
	{
		OwnCharacter.spriteRenderer.flipX = isFlip;
		_playerEquipment.SetEquipFilp(!isFlip);
	}

	void InitPlayerDirection()
	{
		if (OwnCharacter.spriteRenderer.flipX)
		{
			_playerLookDirection = eLookDirection.Left;
		}
		else if (!OwnCharacter.spriteRenderer.flipX)
		{
			_playerLookDirection = eLookDirection.Right;
		}
	}

	public void SetPlayerDirection(eLookDirection direction)
	{
		if (direction == eLookDirection.Left)
		{
			_playerLookDirection = eLookDirection.Left;
			OwnCharacter.spriteRenderer.flipX = true;
			_playerEquipment.SetEquipFilp(true);
		}
		else if (direction == eLookDirection.Right)
		{
			_playerLookDirection = eLookDirection.Right;
			OwnCharacter.spriteRenderer.flipX = false;
			_playerEquipment.SetEquipFilp(false);
		}
	}

	public void PullRope()
	{
		RopePullGame.RopePullRope.Instance.PullRope((int)_playerLookDirection * OwnCharacter.status.strength);
		OwnCharacter.PullRopeAutoRelease();

		_outlineshader.PlayLineEffect();
		AudioManager.Instance.PlaySound(eSoundId.Pull);
	}

	public void NetworkPullRope()
	{
		if (_isHost)
			NetworkManager.Instance.SendRequestRopePull(OwnCharacter.status.strength * -1);
		else
			NetworkManager.Instance.SendRequestRopePull(OwnCharacter.status.strength);

		OwnCharacter.PullRopeAutoRelease();
		_outlineshader.PlayLineEffect();
		AudioManager.Instance.PlaySound(eSoundId.Pull);
	}

	#endregion

	#region Farm Function

	const float MoveSpeed = 1.5f;
	Vector3 _targetPosition;

	LandTile _currentLandTile;
	ProductTile _currentProductTile;
	GarbageTile _currentGarbageTile;

	public void FarmStart()
	{
		Point startPoint = new Point(2, 2);
		MapData.Instance.CurrentFarmerPoint = startPoint;
		FarmUIManager.Instance.harvestButtonPressed += HarvestCurrentProduct;
		FarmUIManager.Instance.cleaningGarbageButtonPressed += CleaningCurrentGarbage;

		transform.position = new Vector3(0.96f, 0.96f, -2.0f);
		transform.localScale = new Vector3(0.25f, 0.25f, 1.0f);
		SyncCameraPosition();
	}

	public void FarmUpdate()
	{
		switch (_farmState)
		{
			case eFarmState.Idle:
				break;
			case eFarmState.Move:
				MoveToTargetPosition();
				CheckStopMove();
				SyncCameraPosition();
				break;
			case eFarmState.Acting:
				break;
			default:
				break;
		}
	}

	public void SetTargetPosition(LandTile landTile)
	{
		OwnCharacter.StartRun();
		if (FriendFarmManager.Instance == null)
		{
			LeaveTile();
		}
		_targetPosition = landTile.transform.position;
		_targetPosition.z = transform.localPosition.z;
		_currentLandTile = landTile;
		_farmState = eFarmState.Move;

		if (OwnCharacter.transform.position.x < _targetPosition.x)
		{
			SetFlipCharacter(false);
		}
		else
		{
			SetFlipCharacter(true);
		}
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
			_farmState = eFarmState.Idle;
			EnterTile();
			return;
		}
	}

	void SyncCameraPosition()
	{
		Vector3 cameraPosition = mainCam.transform.position;

		cameraPosition.x = transform.position.x;
		cameraPosition.y = transform.position.y;

		mainCam.transform.position = cameraPosition;
	}

	void EnterTile()
	{
		OwnCharacter.EndRun();

		if (FriendFarmManager.Instance != null)
		{
			return;
		}

		Point currentPoint = _currentLandTile.MapPoint;
		MapData.Instance.CurrentFarmerPoint = currentPoint;
		_currentLandTile.Highlight = true;

		_currentProductTile = ObjectTileManager.Instance.GetProductTileAtPoint(currentPoint);

		float grownSpeed = 0.0f;
		switch (_currentLandTile.Type)
		{
			case LandTile.BadlandType:
				grownSpeed = LandTile.BadlandGrownSpeedScale;
				break;
			case LandTile.GrassType:
				grownSpeed = LandTile.GrassGrownSpeedScale;
				break;
			case LandTile.CultivateType:
				grownSpeed = LandTile.CultivateGrownSpeedScale;
				break;
			default:
				Debug.LogWarning("Unknown Land Type!");
				break;
		}
		FarmUIManager.Instance.UpdateLandTileChangeInteract(_currentLandTile.Type);

		if (ObjectTileManager.Instance.HasObjectTileAtPoint(currentPoint))
		{
			if (ObjectTileManager.Instance.GetObjectTileTypeAtPoint(currentPoint) == ObjectTile.ETileType.Garbage)
			{
				_currentGarbageTile = ObjectTileManager.Instance.GetGarbageTileAtPoint(currentPoint);
				FarmUIManager.Instance.GarbageTileFuncButtonInteract(_currentGarbageTile.GetGarbageData());
			}
			else
			{
				FarmUIManager.Instance.ObjectTileFuncButtonInteract(false);
			}
		}
		else
		{
			FarmUIManager.Instance.UpdatePlantProductEffectText(grownSpeed);
			FarmUIManager.Instance.ObjectTileFuncButtonInteract(true);
		}

		if (_currentProductTile != null)
		{
			// 수확 기능
			if (_currentProductTile.CanHarvest)
			{
				FarmUIManager.Instance.HarvestButtonActive = true;
			}
			else
			{
				StartCoroutine(CheckCanHarvestEverySeconds());
			}

			// 상단 
			FarmUIManager.Instance.ProductInfoGroupActive = true;
			FarmUIManager.Instance.SetProductInfoData(_currentProductTile.ProductData, grownSpeed);
		}

	}

	void LeaveTile()
	{
		if (_currentLandTile != null)
		{
			_currentLandTile.Highlight = false;
			FarmUIManager.Instance.HarvestButtonActive = false;
			FarmUIManager.Instance.ProductInfoGroupActive = false;
			StopCoroutine(CheckCanHarvestEverySeconds());
		}
		FarmUIManager.Instance.GarbageTileFuncGroupInactive();
	}

	IEnumerator CheckCanHarvestEverySeconds()
	{
		while (true)
		{
			if (_currentProductTile == null)
			{
				yield break;
			}

			if (_currentProductTile.CanHarvest)
			{
				FarmUIManager.Instance.HarvestButtonActive = true;
				yield break;
			}
			else
			{
				yield return new WaitForSeconds(1.0f);
			}
		}
	}

	void HarvestCurrentProduct()
	{
		if (_currentProductTile != null)
		{
			if (_currentProductTile.CanHarvest)
			{
				_currentProductTile.HarvestProduct();
			}
		}
	}

	void CleaningCurrentGarbage()
	{
		if (_currentGarbageTile != null)
		{
			int removeCost = _currentGarbageTile.GetGarbageData().removeCost;
			if (removeCost <= ResourceManager.Instance.GetGoldResource())
			{
				ObjectTileManager.Instance.RemoveObjectTile();
				ResourceManager.Instance.AddGoldResource(-removeCost);
				FarmUIManager.Instance.UpdateGoldResourceLabel();
			}
		}
	}
	#endregion

}
