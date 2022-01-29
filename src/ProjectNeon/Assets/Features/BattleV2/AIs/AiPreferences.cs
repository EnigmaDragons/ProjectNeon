using System;

[Serializable]
public class AiPreferences
{
    public CardTag[] CardTagPriority = new CardTag[0];
    public CardTag[] RotatePlayingCardTags = new CardTag[0];
    public DesignatedAttackerPriority DesignatedAttackerPriority = DesignatedAttackerPriority.Default;
}
