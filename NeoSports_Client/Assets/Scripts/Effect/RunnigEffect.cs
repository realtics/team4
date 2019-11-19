using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnigEffect : MonoBehaviour
{
    public void StartEffect()
    {
        Debug.Log("call");
        gameObject.SetActive(true);
    }

    public void EndEffect()
    {
        Debug.Log("endcall");
        gameObject.SetActive(false);
    }
}
