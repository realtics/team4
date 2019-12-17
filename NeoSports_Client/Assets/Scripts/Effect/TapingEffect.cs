using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            if (!EventSystem.current.IsPointerOverGameObject() &&Input.GetMouseButtonUp(0) == true) 
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
