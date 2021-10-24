using System.Collections.Generic;

public enum CharacterReactionType
{
    Unknown = 0,
    ChainCardPlayed = 1,
    BonusCardPlayed = 2,
    ReactionCardPlayed = 3,
    Blinded = 10,
    Stunned = 11,
    Inhibited = 12,
    Dodged = 13,
    Aegised = 14,
    DoubleDamaged = 15,
    LeftBattle = 16,
    SpawnedIntoBattle = 17
}

public static class CharacterReactionTypeDisplayWords
{
    public static readonly Dictionary<CharacterReactionType, string> DisplayWords =
        new Dictionary<CharacterReactionType, string>
        {
            {CharacterReactionType.ChainCardPlayed, "Chain!"},
            {CharacterReactionType.BonusCardPlayed, "Bonus Card!"},
            {CharacterReactionType.ReactionCardPlayed, "Reaction!"},
            {CharacterReactionType.Blinded, "Blinded"},
            {CharacterReactionType.Stunned, "Stunned"},
            {CharacterReactionType.Inhibited, "Inhibited"},
            {CharacterReactionType.Dodged, "Dodged!"},
            {CharacterReactionType.Aegised, "Prevented!"},
            {CharacterReactionType.DoubleDamaged, "Double Damage!"},
            {CharacterReactionType.LeftBattle, "Is Fleeing!"},
            {CharacterReactionType.SpawnedIntoBattle, "Just Appeared!"}
        };

    public static string DisplayWord(this CharacterReactionType reactionType) =>
        DisplayWords.ValueOrDefault(reactionType, () => reactionType.ToString().WithSpaceBetweenWords());
}