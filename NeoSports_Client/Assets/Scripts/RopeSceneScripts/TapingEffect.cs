using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapingEffect : MonoBehaviour
{
    public Camera mainCam;
    private ParticleSystem effectParticle;
    private Transform selfTransform;

    void Start()
    {
      effectParticle = this.GetComponent<ParticleSystem>();
      selfTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) == true ) // mobile로 변경시 touch 이벤트 추가.
        {
            if (effectParticle != null)
            {
                selfTransform.position = mainCam.ScreenToWorldPoint(Input.mousePosition);
                effectParticle.Play();       
            }
        }  
    }
}
