﻿public class CardActionAvoided
{
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }
    public AvoidanceType AvoidanceType { get; }
    public Member[] AvoidingMembers { get; }

    public CardActionAvoided(EffectData effect, Member source, Target target, AvoidanceType avoidanceType, Member[] avoidingMembers)
    {
        Effect = effect;
        Source = source;
        Target = target;
        AvoidanceType = avoidanceType;
        AvoidingMembers = avoidingMembers;
    }
}
