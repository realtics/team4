using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpirteOutlineshader : MonoBehaviour
{
	public Color color = Color.white;

	[Range(0, 16)]
	public int outlineSize = 1;

	private SpriteRenderer _spriteRenderer;

	void OnEnable()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();

		UpdateOutline(true);
	}

	void OnDisable()
	{
		UpdateOutline(false);
	}

	void Update()
	{
		UpdateOutline(true);
	}

	public void PlayWalkEffect()
	{
		outlineSize = 6;
		color = Color.yellow;
	}
	public void StopWalkEffect()
	{
		outlineSize = 0;
		color = Color.white;
	}

	public void PlayLineEffect()
	{
		outlineSize += 2;
		Invoke("StopLineEffect", 0.5f);
	}

	public void StopLineEffect()
	{
		if(outlineSize > 0)
		outlineSize -=2; 
	}

	void UpdateOutline(bool outline)
	{
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		_spriteRenderer.GetPropertyBlock(mpb);
		mpb.SetFloat("_Outline", outline ? 1f : 0);
		mpb.SetColor("_OutlineColor", color);
		mpb.SetFloat("_OutlineSize", outlineSize);
		_spriteRenderer.SetPropertyBlock(mpb);
	}
}
