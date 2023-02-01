﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterCreatorAnimationController : OnMessage<CharacterAnimationRequested2>
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform character;
    [SerializeField] private PartyAdventureState partyAdventureState;
    [SerializeField] private BattleState state;
    
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
    private bool _canAnimate = true;
    
    public void Init(int memberId, CharacterAnimations characterAnimations, TeamType team)
    {
        _memberId = memberId;
        _team = team;
        _characterAnimations = characterAnimations;
        _currentStates = new Dictionary<int, string>();

        if (animator != null && animator.gameObject.GetComponent<CharacterSoundsAnimationBinding>() == null)
            animator.gameObject.AddComponent<CharacterSoundsAnimationBinding>();
        if (gameObject.GetComponentInChildren<CharacterAnimationSoundPlayer>() == null)
            gameObject.AddComponent<CharacterAnimationSoundPlayer>();
        
        // TODO: Temp Fix for Layering. Doesn't really belong in this class
        InitCharacterSortingLayer();

        if (partyAdventureState == null)
            Log.Error($"{nameof(CharacterCreatorAnimationController)} {nameof(partyAdventureState)} is null");
        if (state == null)
            Log.Error($"{nameof(CharacterCreatorAnimationController)} {nameof(state)} is null");
        if (character == null)
            Log.Error($"{nameof(CharacterCreatorAnimationController)} {nameof(character)} is null");
        if (state == null || partyAdventureState == null || character == null || characterAnimations == null)
        {
            Log.Warn($"{nameof(CharacterCreatorAnimationController)} - {team} {memberId} - Cannot Animate");
            _canAnimate = false;
        }

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

        Log.Info($"Received Character Animation Requested Message for {msg.MemberId}");
        if (!_canAnimate)
        {
            Message.Publish(new Finished<CharacterAnimationRequested2> { Message = msg });
            return;
        }

        if (msg.Condition.IsPresent)
        {
            var ctx = new EffectContext(msg.Source, msg.Target, msg.Card, msg.XPaidAmount, msg.PaidAmount, partyAdventureState, state.PlayerState, state.RewardState,
                state.Members, state.PlayerCardZones, new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), state.CreditsAtStartOfBattle, 
                state.Party.Credits, state.Enemies.ToDictionary(x => x.Member.Id, x => (EnemyType)x.Enemy), () => state.GetNextCardId(), 
                state.CurrentTurnCardPlays(), state.OwnerTints, state.OwnerBusts, false, ReactionTimingWindow.NotApplicable, new EffectScopedData());
            var reasonToNotApply = msg.Condition.Value.GetShouldNotApplyReason(ctx);
            if (reasonToNotApply.IsPresent)
            {
                Log.Info($"Not Starting Character Animation Requested Message for {msg.MemberId} - {reasonToNotApply.Value}");
                Message.Publish(new Finished<CharacterAnimationRequested2> { Message = msg });
                return;
            }
        }
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
        ReturnToDefault();
        _currentCoroutine = FinishAnimationInTime(msg);
        
        Log.Info($"Starting Character Animation Requested Message for {msg.MemberId}");
        this.SafeCoroutineOrNothing(_currentCoroutine);
    }

    private IEnumerator FinishAnimationInTime(CharacterAnimationRequested2 msg)
    {
        var currentAnimation = _characterAnimations.AnimationMap[msg.Animation];
        foreach (var step in currentAnimation)
        {
            if (step.StepType == CharacterAnimationStepType.PublishFinished)
                Message.Publish(new Finished<CharacterAnimationRequested2> { Message = msg });
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
        if (currentAnimation.None(x => x.StepType == CharacterAnimationStepType.PublishFinished))
            Message.Publish(new Finished<CharacterAnimationRequested2> { Message = msg });
        ReturnToDefault();
    }

    private void ReturnToDefault()
    {
        if (!_canAnimate)
            return;
        
        SetAnimatorFloat("Aim", _characterAnimations.Aim);
        SetAnimatorBool(_characterAnimations.Idle, true);
        SetAnimatorBool(_characterAnimations.AimIdle, true);
        foreach (var s in _currentStates)
            SetAnimatorBool(s.Value, false);
        _currentStates.Clear();
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
    
    private void InitCharacterSortingLayer()
    {
        if (animator == null || animator.gameObject == null)
            return;
        
        var obj = animator.gameObject;
        var sort = obj.GetComponent<SortingGroup>();
        if (sort == null) 
            return;
        
        var layerId = SortingLayer.NameToID("Character");
        sort.sortingLayerID = layerId;
    }
}