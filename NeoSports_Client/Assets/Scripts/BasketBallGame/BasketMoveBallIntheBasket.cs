using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketMoveBallIntheBasket : MonoBehaviour
{
    public GameObject goalInBallManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.SetParent(goalInBallManager.transform);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.transform.SetParent(null);
    }

}
