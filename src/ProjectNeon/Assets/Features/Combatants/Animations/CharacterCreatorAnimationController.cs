using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CharacterCreator2D.UI;
using UnityEngine;

public class CharacterCreatorAnimationController : OnMessage<CharacterAnimationRequested2>
{
    [SerializeField] private Animator animator;
    
    private int _memberId;
    private CharacterAnimations _characterAnimations;
    private IEnumerator _currentCoroutine;
    private Dictionary<int, string> _currentStates;

    public void Init(int memberId, CharacterAnimations characterAnimations)
    {
        _memberId = memberId;
        _characterAnimations = characterAnimations;
        _currentStates = new Dictionary<int, string>();
        ReturnToDefault();
    }
    
    protected override void Execute(CharacterAnimationRequested2 msg)
    {
        if (msg.MemberId != _memberId)
            return;
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        ReturnToDefault();
        _currentCoroutine = FinishAnimationInTime(msg);
        StartCoroutine(_currentCoroutine);
    }

    private IEnumerator FinishAnimationInTime(CharacterAnimationRequested2 msg)
    {
        foreach (var step in _characterAnimations.AnimationMap[msg.Animation])
        {
            if (step.StepType == CharacterAnimationStepType.PublishFinished)
                Message.Publish(new Finished<CharacterAnimationRequested>());
            if (step.StepType == CharacterAnimationStepType.Aim)
                animator.SetFloat("Aim", step.Aim);
            if (step.StepType == CharacterAnimationStepType.ChangeState)
            {
                if (step.Layer == 0)
                    animator.SetBool(_characterAnimations.Idle, false);
                if (_currentStates.ContainsKey(step.Layer))
                    animator.SetBool(_currentStates[step.Layer], false);
                animator.SetBool(step.Name, true);
                _currentStates[step.Layer] = step.Name;
            }
            if (step.StepType == CharacterAnimationStepType.ReturnToDefault)
                ReturnToDefault();
            yield return new WaitForSeconds(step.Seconds);
        }
    }

    private void ReturnToDefault()
    {
        animator.SetFloat("Aim", _characterAnimations.Aim);
        animator.SetBool(_characterAnimations.Idle, true);
        foreach (var state in _currentStates)
            animator.SetBool(state.Value, false);
        _currentStates = new Dictionary<int, string>();
    }
}