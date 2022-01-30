using System;
using System.Linq;
using System.Text;

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

    private string TagsDesc(CardTag[] tags) => string.Join(", ", tags);
    
    public string GetCustomizationDescription()
    {
        var sb = new StringBuilder();
        if (CardTagPriority.Length > 0)
            sb.Append($"| Priority: {TagsDesc(CardTagPriority)} ");
        if (RotatePlayingCardTags.Length > 0)
            sb.Append($"| Rotate: {TagsDesc(RotatePlayingCardTags)} ");
        if (UnpreferredCardTags.Length > 0)
            sb.Append($"| Unpreferred: {TagsDesc(UnpreferredCardTags)} ");
        if (CardOrderPreferenceFactor != 0)
            sb.Append($"| CardOrderFactor: {CardOrderPreferenceFactor} ");
        if (DesignatedAttackerPriority != DesignatedAttackerPriority.Default)
            sb.Append($"| DesignatedAttackerPriority: {DesignatedAttackerPriority} ");
        var final = sb.ToString();
        return final.Length == 0 ? "| Default |" : final + " |";
    }

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
