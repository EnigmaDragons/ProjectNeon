
public interface PreventionContext
{
    Member[] GetPreventingMembersRelevantForDamageEffect();
    void UpdatePreventionCounters();
}
