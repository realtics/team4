using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketMove : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed;
    private float _changeTimer;
    private float _direction;
    
    void Start()
    {
        _direction = -1;
        StartCoroutine(MoveBasket());
    }

    IEnumerator MoveBasket()
    {
        while (true)
        {
            ChangeDiretion();
            MoveBasketAuto();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void MoveBasketAuto()
    {
        transform.Translate(new Vector3(_direction * Time.deltaTime * moveSpeed,0.0f,0.0f));
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
