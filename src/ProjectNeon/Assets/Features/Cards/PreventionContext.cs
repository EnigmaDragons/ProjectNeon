
public interface PreventionContext
{
    void RecordPreventionTypeEffect(PreventionType type, params Member[] members);
    Member[] GetPreventingMembers(PreventionType type);
    void UpdatePreventionCounters();
}
