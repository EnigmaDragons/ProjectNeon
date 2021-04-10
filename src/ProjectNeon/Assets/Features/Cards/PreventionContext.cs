
public interface PreventionContext
{
    void RecordPreventionTypeEffect(PreventionType type, Member[] members);
    Member[] GetPreventingMembers(PreventionType type);
    void UpdatePreventionCounters();
}
