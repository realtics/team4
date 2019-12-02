using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasketBallGame
{
    public class BasketAIThrowBall : MonoBehaviour
	{
        const float AIShootRangeMinX = 2.0f;
        const float AIShootRangeMaxX = 15.0f;
        const float AIShootRangeMinY = -15.0f;
        const float AIShootRangeMaxY = 0.0f;

        const float DirectionArrowOffset = 0.5f;

        public float aiFireSpeed;
        public float aiActiveFrequency;
        public GameObject prefAiThrowBall;
        public GameObject directionArrow;
		public BasketBall baksetballPrefab;

		float _powerSize;
        float _arrowScaleOffset;
        float _powerSizeOffset;

		PoolFactory _ballFactory;

		void Start()
        {
            _powerSize = 0.0f;
            _powerSizeOffset = 0.1f;
            _arrowScaleOffset = 2.0f;
			directionArrow.transform.position = new Vector2(transform.position.x - DirectionArrowOffset, transform.position.y + DirectionArrowOffset);

			_ballFactory = new PoolFactory(baksetballPrefab);
			StartCoroutine(UpdateAI());
		}

		IEnumerator UpdateAI()
		{
			while (true)
			{
				if (BasketBallGameManager.Instance.GameState == BasketBallGameManager.EGameState.Playing)
				{
					CalculateAIAngle();
					Fire();
				}
				yield return new WaitForSeconds(aiActiveFrequency);
            }
        }

        void CalculateAIAngle()
        {
            Vector2 target = CalculateTargetPos();

            float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

            directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);

            float power = Vector2.Distance(new Vector2(target.x, target.y), transform.position);

            _powerSize = power * _powerSizeOffset;
            directionArrow.transform.localScale = new Vector3(_powerSize * _arrowScaleOffset, _powerSize * _arrowScaleOffset);
        }

        Vector2 CalculateTargetPos()
        {
            Vector2 target = new Vector2
            {
                x = Random.Range(AIShootRangeMinX, AIShootRangeMaxX),
                y = Random.Range(AIShootRangeMinY, AIShootRangeMaxY)
            };

            return target;
        }

        public void Fire()
        {
            Vector2 direction = directionArrow.transform.rotation * new Vector2(aiFireSpeed, 0.0f) * _powerSize;
            _powerSize = 0.0f;

			//To DO : Instatniate 말고 pool화. 
            //GameObject cannon = Instantiate(prefAiThrowBall, transform.position, transform.rotation);
            //cannon.GetComponent<BasketBall>().ShotToTarget(direction);

			BasketBall ball = _ballFactory.Get() as BasketBall;
			ball.transform.position = transform.position;
			ball.ShotToTarget(direction);
		}

    }
}
