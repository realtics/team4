using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalInTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
        //collision.gameObject.GetComponent<Rigidbody2D>().Sleep();
    }

}
