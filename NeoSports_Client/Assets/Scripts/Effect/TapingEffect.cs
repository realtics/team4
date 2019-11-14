using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapingEffect : MonoBehaviour
{
    private ParticleSystem _efftectParticle;
    private Transform _selftTransform;

    void Start()
    {
      _efftectParticle = GetComponent<ParticleSystem>();
      _selftTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) == true ) // mobile로 변경시 touch 이벤트 추가.
        {
            _selftTransform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (_efftectParticle != null)
            {
                _efftectParticle.Play();       
            }
        }  
    }
}
