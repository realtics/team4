using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketCalculateScore : MonoBehaviour
{
    // Start is called before the first frame update
    public Text leftText;
    public Text rightText;

    private List<Transform> _goalList = new List<Transform>();
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
        GetComponentsInChildren<Transform>(true, _goalList);

        if (_goalList.Count != 0)
        {
            int leftCount = 0;
            int rightCount = 0;

            foreach (Transform ball in _goalList)
            {
                if (ball.CompareTag("Ball"))
                {
                    ++leftCount;
                }
                if (ball.CompareTag("AIBall"))
                {
                    ++rightCount;
                }
            }

            leftText.text = leftCount.ToString();
            rightText.text = rightCount.ToString();
        }
    }

}
