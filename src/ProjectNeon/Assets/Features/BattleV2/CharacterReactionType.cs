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
    SpawnedIntoBattle = 17,
    TookZeroDamage = 18,
}

public static class CharacterReactionTypeDisplayWords
{
    public static string DisplayTerm(this CharacterReactionType reactionType) 
        => $"Reactions/{reactionType.ToString()}_DisplayWords";
}