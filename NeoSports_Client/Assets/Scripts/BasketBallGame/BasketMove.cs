using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketMove : MonoBehaviour
{
    public float moveSpeed;
    public float changeDirectionSecond;

    enum EMoveDirection
    {
        Left = -1,
        Right = 1,
    }
    const int ChangeDirectionNumber = -1;

    private float _changeTimer;
    private float _countSec;
    private int _direction;


    void Start()
    {
        _countSec = 0.1f;
        _direction = (int)EMoveDirection.Left;

        StartCoroutine(DecideMoveDirection());
    }

    void FixedUpdate()
    {
        MoveBasketAuto();
    }

    IEnumerator DecideMoveDirection()
    {
        while (true)
        {
            TickChangeTimer();
            ChangeDirection();
            yield return new WaitForSeconds(_countSec);
        }
    }

    void MoveBasketAuto()
    {
        transform.Translate(new Vector3(_direction * Time.deltaTime * moveSpeed, 0.0f, 0.0f));
    }

    void ChangeDirection()
    {
        if (_changeTimer >= changeDirectionSecond)
        {
            _direction *= ChangeDirectionNumber;
            _changeTimer = 0.0f;
        }
    }

    void TickChangeTimer()
    {
        _changeTimer += _countSec;
    }
}
