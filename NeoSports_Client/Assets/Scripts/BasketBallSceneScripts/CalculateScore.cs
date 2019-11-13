using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateScore : MonoBehaviour
{
    // Start is called before the first frame update
    public Text leftText;
    public Text rightText;

    private GameObject[] _goalInArr;
    void Start()
    {
        StartCoroutine(SecondCount());
    }

    IEnumerator SecondCount()
    {
        while (true)
        {
           UpdateScore();
           yield return new WaitForSeconds(1.0f);
        }
    }

    void UpdateScore()
    {
        //_goalInArr = GetComponentsInChildren<GameObject>();
        //leftText.text = _goalInArr.GetLength();
    }

}
