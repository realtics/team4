using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasketBallGame
{
    public class BasketPlayerMove : MonoBehaviour
    {
        public float speed;

        void Update()
        {
            InputUpdate();
        }

        void InputUpdate()
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
}
