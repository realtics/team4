﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBallIntheBasket : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject goalInBallManager;

    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.transform.SetParent(goalInBallManager.transform);
    }
}
