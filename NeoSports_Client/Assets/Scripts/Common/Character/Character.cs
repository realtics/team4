using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterStatus
{
	public float agility;
	public float endurance;
	public float luck;
	public float strength;
}
public class Character : MonoBehaviour
{
    protected class AnimationParameter
    {
        public const string IsJump = "IsJump";
        public const string JumpUp = "JumpUp";
        public const string IsRun = "IsRun";
    }
	public enum EState
	{
		Idle,
		Jump,
		Run
	}

	public CharacterStatus status;
	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	EState _currentState;
	Animator _animator;
	float _relaseTime = 0.3f;
	public EState CurrentState
	{
		get { return _currentState; }
	}

    private void Awake()
    {
        _currentState = EState.Idle;
        _animator = transform.GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		LoadStatus();
	}

	void LoadStatus()
	{
		if (InventoryManager.Instance != null)
		{
			status.agility = InventoryManager.Instance.CurrentCharacter.Stat.agility;
			status.endurance = InventoryManager.Instance.CurrentCharacter.Stat.endurance;
			status.luck = InventoryManager.Instance.CurrentCharacter.Stat.luck;
			status.strength = InventoryManager.Instance.CurrentCharacter.Stat.strength;
		}
		else
		{
			status.agility = 1;
			status.endurance = 1;
			status.luck = 1;
			status.strength = 1;
		}
	}


	#region CharacterRender
	public void StartJump()
    {
        if (_currentState != EState.Idle)
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

    public void StartRun()
    {
        _animator.SetBool(AnimationParameter.IsRun, true);
        _currentState = EState.Run;
	}

	public void PullRopeAutoRelease()
	{
		_animator.SetBool(AnimationParameter.IsRun, true);
		_currentState = EState.Run;
		Invoke(nameof(EndRun), _relaseTime);
	}

    public void EndRun()
    {
        _animator.SetBool(AnimationParameter.IsRun, false);
        _currentState = EState.Idle;
	}
	#endregion
}
