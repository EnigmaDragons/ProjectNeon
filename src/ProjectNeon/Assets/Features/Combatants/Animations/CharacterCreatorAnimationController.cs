using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreatorAnimationController : OnMessage<CharacterAnimationRequested2>
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform character;
    
    private int _memberId;
    private TeamType _team;
    private CharacterAnimations _characterAnimations;
    private IEnumerator _currentCoroutine;
    private Dictionary<int, string> _currentStates;
    private bool _initializedPosition;
    private Vector3 _start;
    private Vector3 _source;
    private Vector3 _destination;
    private float _totalSeconds;
    private float _secondsRemaining;

    public void Init(int memberId, CharacterAnimations characterAnimations, TeamType team)
    {
        _memberId = memberId;
        _team = team;
        _characterAnimations = characterAnimations;
        _currentStates = new Dictionary<int, string>();
        ReturnToDefault();
    }

    private void Update()
    {
        if (_totalSeconds == 0f)
            return;
        _secondsRemaining = Math.Max(0, _secondsRemaining - Time.deltaTime);
        character.localPosition = Vector3.Lerp(_destination, _source, _secondsRemaining / _totalSeconds);
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
                SetAnimatorFloat("Aim", step.Aim);
            if (step.StepType == CharacterAnimationStepType.ChangeState)
            {
                if (step.Layer == 0)
                    SetAnimatorBool(_characterAnimations.Idle, false);
                if (step.Layer == 1)
                    SetAnimatorBool(_characterAnimations.AimIdle, false);
                if (_currentStates.ContainsKey(step.Layer))
                    SetAnimatorBool(_currentStates[step.Layer], false);
                SetAnimatorBool(step.Name, true);
                _currentStates[step.Layer] = step.Name;
            }
            if (step.StepType == CharacterAnimationStepType.ReturnToDefault)
                ReturnToDefault();
            if (step.StepType == CharacterAnimationStepType.Move)
            {
                if (!_initializedPosition)
                {
                    _start = character.localPosition;
                    _initializedPosition = true;
                }
                _source = character.localPosition;
                if (_team == TeamType.Party)
                    _destination = _start + new Vector3(step.X * 5, 0, step.Y * 5);
                else
                    _destination = _start - new Vector3(step.X, 0, step.Y);
                _totalSeconds = step.Seconds;
                _secondsRemaining = step.Seconds;
            }
            if (step.Seconds != 0f)
                yield return new WaitForSeconds(step.Seconds);
        }
    }

    private void ReturnToDefault()
    {
        SetAnimatorFloat("Aim", _characterAnimations.Aim);
        SetAnimatorBool(_characterAnimations.Idle, true);
        SetAnimatorBool(_characterAnimations.AimIdle, true);
        foreach (var state in _currentStates)
            SetAnimatorBool(state.Value, false);
        _currentStates = new Dictionary<int, string>();
        if (!_initializedPosition)
            return;
        _source = _start;
        _destination = _start;
        _totalSeconds = 0f;
        _secondsRemaining = 0f;
        character.localPosition = _start;
    }

    private void SetAnimatorFloat(string name, float value)
    {
        if (animator != null)
            animator.SetFloat(name, _characterAnimations.Aim);
    }

    private void SetAnimatorBool(string name, bool value)
    {
        if (animator != null)
            animator.SetBool(name, value);
    }
}