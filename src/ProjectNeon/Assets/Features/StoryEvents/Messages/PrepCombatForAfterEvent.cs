public class PrepCombatForAfterEvent
{
    public EnterSpecificBattle Battle { get; }
    public PrepCombatForAfterEvent(EnterSpecificBattle battle) => Battle = battle;
}