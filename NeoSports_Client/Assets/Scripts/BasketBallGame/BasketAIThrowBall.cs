using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketAIThrowBall : MonoBehaviour
{
    const float AIShootRangeMinX = 2.0f;
    const float AIShootRangeMaxX = 15.0f;

    const float AIShootRangeMinY = -15.0f;
    const float AIShootRangeMaxY = 0.0f;

    const float DirectionArrowOffset = 0.5f;

    public float aiFireSpeed;
    public float aiActiveFrequency;
    public GameObject directionArrow;

    private float _powerSize;
    private float _arrowScaleOffset;
    private float _powerSizeOffset;

    void Start()
    {
        _powerSize = 0.0f;
        _powerSizeOffset = 0.1f;
        _arrowScaleOffset = 3.0f;
        directionArrow.transform.position = new Vector2(transform.position.x - DirectionArrowOffset, transform.position.y + DirectionArrowOffset);

        StartCoroutine(UpdateAI());
    }

    IEnumerator UpdateAI()
    {
        while (true)
        {
            CalculateAIAngle();
            Fire();
            yield return new WaitForSeconds(aiActiveFrequency);
        }
    }

    private void CalculateAIAngle()
    {
        Vector2 target = CalculateTargetPos();

        float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

        directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);

        float power = Vector2.Distance(new Vector2(target.x, target.y), transform.position);

        _powerSize = power * _powerSizeOffset;
        directionArrow.transform.localScale = new Vector3(_powerSize * _arrowScaleOffset, _powerSize * _arrowScaleOffset);
    }

    private Vector2 CalculateTargetPos()
    {
        Vector2 target = new Vector2();

        target.x = Random.Range(AIShootRangeMinX, AIShootRangeMaxX);
        target.y = Random.Range(AIShootRangeMinY, AIShootRangeMaxY);

        return target;
    }

    public void Fire()
    {
        Vector2 direction = directionArrow.transform.rotation * new Vector2(aiFireSpeed, 0.0f) * _powerSize;
        _powerSize = 0.0f;

        //Fix Me : 프리팹 동적로드 하지말고 캐싱하도록 
        GameObject toInstance = Resources.Load<GameObject>("Prefabs/BasketPrefabs/AIThrowBall");
        GameObject cannon = Instantiate(toInstance, transform.position, transform.rotation);
        cannon.GetComponent<BasketPlayerCannon>().ShotToTarget(direction);
    }

}
