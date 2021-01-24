public interface ResourceCalculator : ITemporalState
{
    void RecordUsageIfApplicable(Card card);
    void UndoUsageIfApplicable(Card card);
    ResourceCalculations GetModifiers(CardTypeData card, MemberState member);
}