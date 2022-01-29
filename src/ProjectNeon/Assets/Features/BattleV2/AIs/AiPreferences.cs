using System;
using System.Linq;

[Serializable]
public class AiPreferences
{
    public CardTag[] CardTagPriority = new CardTag[0];
    public CardTag[] RotatePlayingCardTags = new CardTag[0];
    public CardTag[] UnpreferredCardTags = new CardTag[0];
    public DesignatedAttackerPriority DesignatedAttackerPriority = DesignatedAttackerPriority.Default;

    public AiPreferences WithDefaultsBasedOnRole(BattleRole role)
    {
        if (role == BattleRole.Specialist)
            return CopyWith(x => x.UnpreferredCardTags = UnpreferredCardTags.Append(CardTag.Attack).ToHashSet().ToArray());
        return this;
    }

    private AiPreferences CopyWith(Action<AiPreferences> update)
    {
        var clone = new AiPreferences
        {
            CardTagPriority = CardTagPriority,
            RotatePlayingCardTags = RotatePlayingCardTags,
            UnpreferredCardTags = UnpreferredCardTags.Append(CardTag.Attack).ToHashSet().ToArray(),
            DesignatedAttackerPriority = DesignatedAttackerPriority
        };
        update(clone);
        return clone;
    }
}
