using System;

[Serializable]
public class CharacterAnimationStep
{
    public CharacterAnimationStepType StepType;
    public StringReference Name;
    public int Layer;
    public float Aim;
    public float Seconds;
}