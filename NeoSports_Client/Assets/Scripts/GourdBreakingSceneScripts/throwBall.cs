using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*각 계산하는 부분 인터넷 참고 하였습니다.*/
public class throwBall : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float fireSpeed; 
    public GameObject directionArrow;
    

    private Collider2D _ownCollider;
    private bool _isTargetting;
    private float _startAngle;
    private float _powerSize;
    private float _arrowScaleOffset;
    private float _powerSizeOffset;

    void Start()
    {
        _isTargetting = false;
        _startAngle = 0.0f;
        _ownCollider = GetComponent<Collider2D>();
        _powerSize = 0.0f;
        _powerSizeOffset = 0.1f;
        _arrowScaleOffset = 3.0f;
    }

    void Update()
    {
        ShotInWindow();
    }

    private void ShotInWindow()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_ownCollider.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                directionArrow.transform.position = new Vector2(transform.position.x+0.3f, transform.position.y + 0.5f);
                directionArrow.transform.localScale = new Vector3(3, 3, 3);
                _isTargetting = true;
                _startAngle = transform.eulerAngles.z;
            }
        }
        else if (Input.GetMouseButton(0) && _isTargetting)
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(transform.position.y - target.y, transform.position.x - target.x);
            float angleDiff = DifferenceBetweenAngles(angle, _startAngle);
            
            if (angle < 1.6f && angle > -0.7f)
            {
                directionArrow.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, transform.forward);
            }

            float power = Vector2.Distance(target, transform.position);

            _powerSize = power * _powerSizeOffset ;
            directionArrow.transform.localScale = new Vector3(_powerSize * _arrowScaleOffset, _powerSize * _arrowScaleOffset);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            directionArrow.transform.localScale = new Vector3(0, 0, 0);
            _isTargetting = false;
            Fire();
        }
    }

    private float DifferenceBetweenAngles(float angle1, float angle2)
    {
        float angle = angle1 - angle2;
        return Mathf.Atan2(Mathf.Sin(angle), Mathf.Cos(angle));
    }

    public void Fire()
    {
        Vector2 direction = directionArrow.transform.rotation * new Vector2(fireSpeed, 0.0f) * _powerSize;
        _powerSize = 0.0f;

        GameObject toInstance = Resources.Load<GameObject>("Prefabs/BasketPrefabs/ThrowBall");
        GameObject cannon = Instantiate(toInstance, transform.position, transform.rotation);
        cannon.GetComponent<PlayerCannon>().ShotToTarget(direction);

    }


}
