using System;
using System.Linq;

[Serializable]
public class AiPreferences
{
    public DesignatedAttackerPriority DesignatedAttackerPriority = DesignatedAttackerPriority.Default;
    public int CardOrderPreferenceFactor = 0;
    public CardTag[] CardTagPriority = new CardTag[0];
    public CardTag[] UnpreferredCardTags = new CardTag[0];
    public CardTag[] RotatePlayingCardTags = new CardTag[0];

    public bool IsDefault => CardTagPriority.Length == 0
                             && UnpreferredCardTags.Length == 0
                             && RotatePlayingCardTags.Length == 0
                             && CardOrderPreferenceFactor == 0
                             && DesignatedAttackerPriority == DesignatedAttackerPriority.Default;

    public AiPreferences WithDefaultsBasedOnRole(BattleRole role)
    {
        if (role == BattleRole.Specialist)
            return CopyWith(x =>
            {
                x.UnpreferredCardTags = UnpreferredCardTags.Append(CardTag.Attack).ToHashSet().ToArray();
                if (x.DesignatedAttackerPriority == DesignatedAttackerPriority.Default)
                    x.DesignatedAttackerPriority = DesignatedAttackerPriority.IsNotDamageDealer;
            });
        if (role == BattleRole.Survivability)
            return CopyWith(x =>
            {
                if (x.DesignatedAttackerPriority == DesignatedAttackerPriority.Default)
                    x.DesignatedAttackerPriority = DesignatedAttackerPriority.IsNotDamageDealer;
            });
        return this;
    }

    private AiPreferences CopyWith(Action<AiPreferences> update)
    {
        var clone = new AiPreferences
        {
            CardTagPriority = CardTagPriority,
            RotatePlayingCardTags = RotatePlayingCardTags,
            UnpreferredCardTags = UnpreferredCardTags.Append(CardTag.Attack).ToHashSet().ToArray(),
            DesignatedAttackerPriority = DesignatedAttackerPriority,
            CardOrderPreferenceFactor = 1
        };
        update(clone);
        return clone;
    }
}
