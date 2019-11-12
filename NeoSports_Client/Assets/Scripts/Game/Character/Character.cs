using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

	protected class AnimationParameter
	{
		public const string IsJump = "IsJump";
		public const string JumpUp = "JumpUp";
	}

	public enum EState
	{
		Idle,
		Jump
	}

	EState currentState;
	Animator animator;

	Vector3 originalPosition;
	float jumpProgress;

	public EState CurrentState {
		get { return currentState; }
	}

	private void Start()
	{
		currentState = EState.Idle;
		animator = transform.GetComponent<Animator>();
	}

	private void Update()
	{
		//UpdateAnimation();
	}

	void UpdateAnimation()
	{
		if(currentState == EState.Jump)
		{
			jumpProgress += Time.deltaTime;
			if(jumpProgress > 2.0f)
			{
				EndJump();
			}
		}
	}

	public void StartJump()
	{
		if(currentState != EState.Idle)
		{
			return;
		}
		animator.SetBool(AnimationParameter.IsJump, true);
		animator.SetBool(AnimationParameter.JumpUp, true);
		currentState = EState.Jump;
	}

	public void JumpMax()
	{
		animator.SetBool(AnimationParameter.JumpUp, false);
	}

	public void EndJump()
	{
		animator.SetBool(AnimationParameter.IsJump, false);
		currentState = EState.Idle;
	}
}
