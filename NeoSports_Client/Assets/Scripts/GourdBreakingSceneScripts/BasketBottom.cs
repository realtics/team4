using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBottom : MonoBehaviour
{
    private Rigidbody2D _rigidyBody2DColl;
    private BoxCollider2D _boxCollider2DColl;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _rigidyBody2DColl = collision.gameObject.GetComponent<Rigidbody2D>();
            //_rigidyBody2DColl.velocity = new Vector2(0.0f, 0.0f);
            _boxCollider2DColl = collision.gameObject.GetComponent<BoxCollider2D>();
            //_rigidyBody2DColl.freezePosition();
            //_boxCollider2DColl.
            //_rigidyBody2DColl.gravityScale = 0.0f;
            //_rigidyBody2DColl.Sleep();
            
        }
    }
}
