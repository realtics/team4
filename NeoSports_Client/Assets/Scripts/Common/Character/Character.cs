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

	EState		_currentState;
	Animator	_animator;

	public EState CurrentState {
		get { return _currentState; }
	}

	private void Start()
	{
		_currentState = EState.Idle;
		_animator = transform.GetComponent<Animator>();
	}

	public void StartJump()
	{
		if(_currentState != EState.Idle)
		{
			return;
		}
		_currentState = EState.Jump;
		_animator.SetBool(AnimationParameter.IsJump, true);
		_animator.SetBool(AnimationParameter.JumpUp, true);
	}

	public void JumpMax()
	{
		_animator.SetBool(AnimationParameter.JumpUp, false);
	}

	public void EndJump()
	{
		_animator.SetBool(AnimationParameter.IsJump, false);
		_currentState = EState.Idle;
	}
}
