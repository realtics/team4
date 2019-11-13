using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIThrowBall : MonoBehaviour
{
 
    // Start is called before the first frame update
    
    public float aiFireSpeed;
    public float aiActiveFrequency;
    public GameObject directionArrow;
    
    private Collider2D _ownCollider;
    private float _powerSize;
    private float _arrowScaleOffset;
    private float _powerSizeOffset;


    void Start()
    {
        _ownCollider = GetComponent<Collider2D>();
        _powerSize = 0.0f;
        _powerSizeOffset = 0.1f;
        _arrowScaleOffset = 3.0f;
        directionArrow.transform.position = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.5f);
       
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
        Vector2 target = new Vector2(Random.Range(2, 15), Random.Range(0, -15));
        float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);

        directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
        
        float power = Vector2.Distance(new Vector2(target.x,target.y), transform.position);

        _powerSize = power * _powerSizeOffset ;
        directionArrow.transform.localScale = new Vector3(_powerSize * _arrowScaleOffset, _powerSize * _arrowScaleOffset);
    }

    public void Fire()
    {
        Vector2 direction = directionArrow.transform.rotation * new Vector2(aiFireSpeed , 0.0f) * _powerSize;
        _powerSize = 0.0f;

        GameObject toInstance = Resources.Load<GameObject>("Prefabs/BasketPrefabs/AIThrowBall");
        GameObject cannon = Instantiate(toInstance, transform.position, transform.rotation);
        cannon.GetComponent<PlayerCannon>().ShotToTarget(direction);
    }

}
