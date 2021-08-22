
public sealed class EquipmentPersistentState : IPersistentState
{
    private readonly Equipment _equipment;
    private readonly EffectContext _ctx;

    public EquipmentPersistentState(Equipment equipment, EffectContext ctx)
    {
        _equipment = equipment;
        _ctx = ctx;
    }

    public void OnTurnStart()
    {
        _equipment.TurnStartEffects
            .ForEach(e => AllEffects.Apply(e, _ctx.WithFreshPreventionContext()));
    }

    public void OnTurnEnd()
    {
        _equipment.TurnEndEffects
            .ForEach(e => AllEffects.Apply(e, _ctx.WithFreshPreventionContext()));
    }
}
