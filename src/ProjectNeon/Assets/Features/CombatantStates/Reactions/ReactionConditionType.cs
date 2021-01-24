namespace Features.CombatantStates.Reactions
{
    public enum ReactionConditionType
    {
        OnAttacked = 0,
        OnDamaged = 1,
        OnVulnerable = 5,
        OnShieldBroken = 6,
        OnBloodied = 10,
        OnCausedHeal = 15,
        OnCausedStun = 20,
    }
}