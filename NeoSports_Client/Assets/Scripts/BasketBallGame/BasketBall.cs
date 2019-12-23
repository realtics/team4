using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*각 계산하는 부분 인터넷 참고 하였습니다.*/

namespace BasketBallGame
{
	public enum EBallOwner
	{
		EMPTY,
		LeftPlayer,
		RightPlayer,
		AI,
	}

	public class BasketBall : RecycleObject
	{

		public float _lowerLimit;
		//public TrailRenderer trailPrefab;
		public Action<BasketBall> destroyed;

		Rigidbody2D _rb2d;
		Vector3 _prevPosition;
		TrailRenderer _trailEffect;
		bool _isActivated = false;

		EBallOwner _ownerMarking;

		void Awake()
		{
			_trailEffect = GetComponent<TrailRenderer>();
		}
		void OnEnable()
		{
			_rb2d = GetComponent<Rigidbody2D>();
			_prevPosition = transform.position;
			_ownerMarking = EBallOwner.EMPTY;
		}

		void Update()
		{
			CheckOutScreenObject();
		}

		void FixedUpdate()
		{
			if (!_isActivated)
			{
				return;
			}
			MoveToTarget();
		}

		void CheckOutScreenObject()
		{
			if (transform.position.y <= _lowerLimit)
			{
				destroyed(this);
				if (destroyed != null)
				{


				}
			}
		}

		void MoveToTarget()
		{
			Vector3 deltaPos = transform.position - _prevPosition;
			float angle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg;
			if (0 != angle)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), 10.0f * Time.deltaTime);
				_prevPosition = transform.position;
			}
		}

		public void ShotToTarget(Vector2 direction)
		{
			_rb2d.velocity = direction;
			AudioManager.Instance.PlaySound(eSoundId.Shoot);
		}

		public void Activate(Vector3 startPosition, EBallOwner ballMark, string ballTag)
		{
			transform.position = startPosition;
			_isActivated = true;
			_ownerMarking = ballMark;
			tag = ballTag;
			_trailEffect.Clear();
		}

		public void SetActiveTraill(bool isActive)
		{
			_trailEffect.enabled = isActive;
		}
	}
}
