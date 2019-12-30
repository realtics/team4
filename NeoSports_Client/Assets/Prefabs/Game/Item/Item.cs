using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarmGame;

public class Item : RecycleObject
{
    const float RotateSpeed = 50.0f;
    Collider2D _ownCollider2d;
	SpriteRenderer _spriteRenderer;

    public TempEffect _effect;
    public int goldAmount;

	enum eItemCartegory
	{
		Buff,
		DeBuff,
		Gold,
	}

	void Start()
	{
		_ownCollider2d = GetComponent<Collider2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
        

    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * RotateSpeed);
    }

    void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}

	void OnTriggerEnter2D(Collider2D collision)
	{ 
		var player = collision.GetComponent<Player>();
		if (player != null)
		{
			player.getItem();
            _effect.PlayEffect(transform);
            AudioManager.Instance.PlaySound(eSoundId.Score);
            gameObject.SetActive(false); //대신 오브젝트 풀 쓸수 있으면 오브젝트 풀 사용 할 것.
			if (player.ISControlPlayer)
				return;
            if (NetworkManager.Instance != null)
				ResourceManager.Instance.AddGoldResource(goldAmount);

            
        }	
	}
	
}
