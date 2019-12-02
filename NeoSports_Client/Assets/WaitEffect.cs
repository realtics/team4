using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitEffect : MonoBehaviour
{
    // Start is called before the first frame update
	private float _speed;
    void Start()
    {
		_speed = 100.0f;
	}

    // Update is called once per frame
    void Update()
    {
		new Vector3();
		transform.Rotate(-Vector3.forward  *Time.deltaTime * _speed);
    }
}
