using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCannon : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D _rb2d;
    private Vector3 _prevPosition;

    void OnEnable()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _prevPosition = transform.position;
    }

    void Update()
    {
        CheckOutScreenObject();
    }

    void FixedUpdate()
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
    }

    private void CheckOutScreenObject()
    {
        if(transform.position.y < -5.0f)
        {
            DestroyOutScreenObject();
        }
    }
    private void DestroyOutScreenObject()
    {
        Destroy(gameObject, 0.5f);
    }
}
