
public class BattleAttritionTracker
{
    private int _startingNumInjuries;
    private int _startingCredits;
    private int _startingMissingHp;

    private BattleAttritionTracker(int startingMissingHp, int startingCredits, int startingNumInjuries)
    {
        _startingNumInjuries = startingNumInjuries;
        _startingCredits = startingCredits;
        _startingMissingHp = startingMissingHp;
    }

    public static BattleAttritionTracker Start(PartyAdventureState party)
        => new BattleAttritionTracker(party.TotalMissingHp, party.Credits, party.TotalNumInjuries);

    public BattleAttritionReport Finalize(PartyAdventureState party)
        => new BattleAttritionReport(party.TotalMissingHp - _startingMissingHp,
            party.TotalNumInjuries - _startingNumInjuries, party.Credits - _startingCredits);
}