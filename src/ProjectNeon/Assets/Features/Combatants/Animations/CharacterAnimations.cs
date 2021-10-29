using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/CharacterAnimations")]
public class CharacterAnimations : ScriptableObject
{
    public StringReference Idle = new StringReference("Idle");
    public StringReference AimIdle = new StringReference("None");
    public float Aim = 0f;
    [SerializeField] private CharacterAnimation crouch;
    [SerializeField] private CharacterAnimation slash;
    [SerializeField] private CharacterAnimation stab;
    [SerializeField] private CharacterAnimation targetSingleEnemy;
    [SerializeField] private CharacterAnimation targetTeam;
    [SerializeField] private CharacterAnimation selfBuff;
    [SerializeField] private CharacterAnimation pace;
    [SerializeField] private CharacterAnimation hit;
    [SerializeField] private CharacterAnimation shot;
    [SerializeField] private CharacterAnimation rapidShot;

    public DictionaryWithDefault<CharacterAnimationType, CharacterAnimationStep[]> AnimationMap 
        => new DictionaryWithDefault<CharacterAnimationType, CharacterAnimationStep[]>(new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } })
        {
            { CharacterAnimationType.Crouch, crouch?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.Slash, slash?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.Stab, stab?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.TargetSingleEnemy, targetSingleEnemy?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.TargetTeam, targetTeam?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.SelfBuff, selfBuff?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.Pace, pace?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.Hit, hit?.Steps ?? new CharacterAnimationStep[0] },
            { CharacterAnimationType.Shot, shot?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
            { CharacterAnimationType.RapidShot, rapidShot?.Steps ?? new [] { new CharacterAnimationStep { StepType = CharacterAnimationStepType.PublishFinished } } },
        };
}