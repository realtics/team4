using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{

	readonly Rect CloudShowRect = new Rect(-10.0f, -1.0f, 20.0f, 2.0f);

	const string CloudSpriteNamePrefix = "scene_0";
	const string CloudSpriteNameSuffix = "_cloud";
	const int CloudSpriteIndexMin = 1;
	const int CloudSpriteIndexMax = 4;

	const float MoveSpeedMin = 0.5f;
	const float MoveSpeedMax = 1.5f;
	const float Depth = -1.0f;

	Vector3 _position;
	float _moveSpeed;

	private void Start()
	{
		_position = transform.localPosition;

		SetMoveSpeed();
		SetRandomStartPosition();
		SetRandomSprite();
	}

	private void Update()
	{
		Move();
		CheckCanDestroy();
	}

	void Move()
	{
		_position.x += _moveSpeed * Time.deltaTime;

		transform.localPosition = _position;
	}

	void SetMoveSpeed()
	{
		_moveSpeed = Random.Range(MoveSpeedMin, MoveSpeedMax);
	}

	void SetRandomStartPosition()
	{
		_position.x = CloudShowRect.xMin;
		_position.y = Random.Range(CloudShowRect.yMin, CloudShowRect.yMax);
		_position.z = Depth;

		transform.localPosition = _position;
	}

	void SetRandomSprite()
	{
		int spriteIndex = Random.Range(CloudSpriteIndexMin, CloudSpriteIndexMax);
		string spriteName = CloudSpriteNamePrefix + spriteIndex.ToString() + CloudSpriteNameSuffix;

		Sprite sprite = Singleton<ResourceManager>.Instance.GetGameSprite(spriteName);
		transform.GetComponent<SpriteRenderer>().sprite = sprite;
	}

	void CheckCanDestroy()
	{
		if (_position.x > CloudShowRect.xMax)
		{
			Destroy(gameObject);
		}
	}

}
