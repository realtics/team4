using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEffect : MonoBehaviour
{
    public GameObject conffetiPrefab;

    Transform _delayTransform;

    public void PlayEffect(Transform SpawnTrans)
    {
        GameObject ob = Instantiate(conffetiPrefab);
        ob.transform.position = SpawnTrans.position;
        Destroy(ob,2.5f);
    }

}
