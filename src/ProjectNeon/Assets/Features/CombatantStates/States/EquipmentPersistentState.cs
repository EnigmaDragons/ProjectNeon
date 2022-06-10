
public sealed class EquipmentPersistentState : IPersistentState
{
    private readonly Equipment _equipment;
    private readonly EffectContext _ctx;

    public EquipmentPersistentState(Equipment equipment, EffectContext ctx)
    {
        _equipment = equipment;
        _ctx = ctx;
    }

    public IPayloadProvider OnTurnStart()
    {
        return new SinglePayload($"Equipment - {_equipment.Name}", new PerformAction(() =>
        {
            _equipment.TurnStartEffects
                .ForEach(e => AllEffects.Apply(e, _ctx.WithFreshPreventionContext()));
        }));
    }

    public IPayloadProvider OnTurnEnd()
    {       
        return new SinglePayload($"Equipment - {_equipment.Name}", new PerformAction(() =>
        {
            _equipment.TurnEndEffects
                .ForEach(e => AllEffects.Apply(e, _ctx.WithFreshPreventionContext()));
        }));
    }
}
