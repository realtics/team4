using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketMove : MonoBehaviour
{
    public float moveSpeed;
    private float _changeTimer;
    private float _direction;
    //public Rigidbody2D _rigidbody2D; ToDo  : R&D better Way

    void Start()
    {
        _direction = -1;
        StartCoroutine(MoveBasket());
    }
    void FixedUpdate()
    {
        MoveBasketAuto();
    }

    IEnumerator MoveBasket()
    {
        while (true)
        {
            ChangeDiretion();
            //MoveBasketAuto();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void MoveBasketAuto()
    {
        //FixMe
        transform.Translate(new Vector3(_direction * Time.deltaTime * moveSpeed, 0.0f, 0.0f));
        //_rigidbody2D.AddForce(new Vector2(_direction * Time.deltaTime * moveSpeed, 0.0f));
        //ToDo: R & D better Way
    }

    void ChangeDiretion()
    {
        _changeTimer += 0.1f;
        if (_changeTimer >= 5.0f)
        {
            _direction *= -1;
            _changeTimer = 0.0f;
        }
    }
}
