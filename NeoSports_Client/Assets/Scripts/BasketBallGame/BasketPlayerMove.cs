using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketPlayerMove : MonoBehaviour
{
    // Start is called before the first frame update

    public float speed;

    void Update()
    {
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            transform.Translate(new Vector3(-1 * speed * Time.deltaTime, 0f, 0f));
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(1 * speed * Time.deltaTime, 0f, 0f));
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0f, 1 * speed * Time.deltaTime, 0f));
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0f, -1 * speed * Time.deltaTime, 0f));
        }
    }
}
