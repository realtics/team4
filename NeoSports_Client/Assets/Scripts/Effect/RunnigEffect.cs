using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnigEffect : MonoBehaviour
{
    public void StartEffect()
    {
        gameObject.SetActive(true);
    }

    public void EndEffect()
    {
        gameObject.SetActive(false);
    }
}
