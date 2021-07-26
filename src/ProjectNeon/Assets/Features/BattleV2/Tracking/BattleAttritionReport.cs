
public class BattleAttritionReport
{
    public int TotalHpChange { get; }
    public int TotalInjuriesChange { get; }
    public int TotalCreditsChange { get; }

    public BattleAttritionReport(int totalHpChange, int totalInjuriesChange, int totalCreditsChange)
    {
        TotalHpChange = totalHpChange;
        TotalInjuriesChange = totalInjuriesChange;
        TotalCreditsChange = totalCreditsChange;
    }
}