﻿using System;
using UnityEngine;

[Serializable]
public class CharacterAnimations
{
    public StringReference Idle = new StringReference("Idle");
    public StringReference AimIdle = new StringReference("None");
    public float Aim = 0f;
    [SerializeField] private CharacterAnimation crouch;
    [SerializeField] private CharacterAnimation slash;
    [SerializeField] private CharacterAnimation stab;
    [SerializeField] private CharacterAnimation targetSingleEnemy;
    [SerializeField] private CharacterAnimation targetTeam;
    
    public DictionaryWithDefault<CharacterAnimationType, CharacterAnimationStep[]> AnimationMap 
        => new DictionaryWithDefault<CharacterAnimationType, CharacterAnimationStep[]>(new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } })
        {
            { CharacterAnimationType.Crouch, crouch?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.Slash, slash?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.Stab, stab?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.TargetSingleEnemy, targetSingleEnemy?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.TargetTeam, targetTeam?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
        };
}