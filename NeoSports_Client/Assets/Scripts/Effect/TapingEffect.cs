using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effect
{
    public class TapingEffect : MonoBehaviour
    {
        ParticleSystem _efftectParticle;
        Transform _selftTransform;
		Camera _mainCam;

        void Start()
        {
            _efftectParticle = GetComponent<ParticleSystem>();
            _selftTransform = GetComponent<Transform>();
			_mainCam = Camera.main;
		}

        void Update()
        {
            if (Input.GetMouseButtonUp(0) == true) // mobile로 변경시 touch 이벤트 추가.
            {
                _selftTransform.position = _mainCam.ScreenToWorldPoint(Input.mousePosition);

                if (_efftectParticle != null)
                {
                    _efftectParticle.Play();
                }
            }
        }
    }
}
