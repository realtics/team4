using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*각 계산하는 부분 인터넷 참고 하였습니다.*/
namespace BasketBallGame
{
    public class BasketThrowBall : MonoBehaviour 
    {
        const float angleMax = 1.6f;
        const float angleMin = -0.7f;

        public float fireSpeed;
        public GameObject directionArrow;
        public GameObject prefThrowBall;
		public BasketBall baksetballPrefab;

        Collider2D _ownCollider;
        bool _isTargetting;
        float _powerSize;
        float _arrowScaleOffset;
        float _powerSizeOffset;
		Camera _mainCam;
		PoolFactory _ballFactory;

		void Start()
        {
            _isTargetting = false;
            _ownCollider = GetComponent<Collider2D>();
            _powerSize = 0.0f;
            _powerSizeOffset = 0.1f;
            _arrowScaleOffset = 5.0f;
			_mainCam = Camera.main;
			_ballFactory = new PoolFactory(baksetballPrefab);

		}

        void Update()
        {
			if(BasketBallGameManager.Instance.GameState == BasketBallGameManager.EGameState.Playing)
			{
				ShootBallInTouch();
			}
		}

        void ShootBallInTouch()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_ownCollider.OverlapPoint(_mainCam.ScreenToWorldPoint(Input.mousePosition)))
                {
					AimingShoot();
                }
            }
            else if (Input.GetMouseButton(0) && _isTargetting)
            {
				CalculateShoot();
			}
            else if (Input.GetMouseButtonUp(0))
            {
                ShootBall();
            }
        }

        void ShootBall()
        {
			directionArrow.transform.localScale = new Vector3(0, 0, 0);
			_isTargetting = false;

			Vector2 direction = directionArrow.transform.rotation * new Vector2(fireSpeed, 0.0f) * _powerSize;
            _powerSize = 0.0f;

			BasketBall ball = _ballFactory.Get() as BasketBall;
			ball.ShotToTarget(direction);
			ball.Activate(transform.position,EBallOwner.LeftPlayer,"Ball");
			ball.destroyed += OnBallDestroyed;
		}

		void OnBallDestroyed(BasketBall usedBall)
		{
			usedBall.destroyed -= OnBallDestroyed;
			_ballFactory.Restore(usedBall);
		}

		void AimingShoot()
		{
			directionArrow.transform.position = new Vector2(transform.position.x + 0.3f, transform.position.y + 0.5f);
			_isTargetting = true;
		}

		void CalculateShoot()
		{
			Vector2 target = _mainCam.ScreenToWorldPoint(Input.mousePosition);
			float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

			directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);

			float power = Vector2.Distance(target, transform.position);
			_powerSize = power * _powerSizeOffset;
			directionArrow.transform.localScale = new Vector3(_powerSize * _arrowScaleOffset, _powerSize * _arrowScaleOffset);
		}

    }
}
