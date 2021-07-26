
public class BattleAttritionTracker
{
    private readonly int _startingNumInjuries;
    private readonly int _startingCredits;
    private readonly int _startingMissingHp;

    private BattleAttritionTracker(int startingMissingHp, int startingCredits, int startingNumInjuries)
    {
        _startingNumInjuries = startingNumInjuries;
        _startingCredits = startingCredits;
        _startingMissingHp = startingMissingHp;
    }

    public static BattleAttritionTracker Start(PartyAdventureState party)
        => new BattleAttritionTracker(party.TotalMissingHp, party.Credits, party.TotalNumInjuries);

    public BattleAttritionReport Finalize(PartyAdventureState party)
        => new BattleAttritionReport(_startingMissingHp - party.TotalMissingHp,
            party.TotalNumInjuries - _startingNumInjuries, party.Credits - _startingCredits);
}