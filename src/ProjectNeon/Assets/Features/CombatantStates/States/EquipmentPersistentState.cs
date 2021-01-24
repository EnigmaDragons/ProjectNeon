
public sealed class EquipmentPersistentState : IPersistentState
{
    private readonly Equipment _equipment;
    private readonly Member _owner;
   
    public EquipmentPersistentState(Equipment equipment, Member owner)
    {
        _equipment = equipment;
        _owner = owner;
    }

    public void OnTurnStart()
    {
        _equipment.TurnStartEffects
            .ForEach(e => AllEffects.Apply(e, new EffectContext(_owner, new Single(_owner), Maybe<Card>.Missing(), 0)));
    }

    public void OnTurnEnd()
    {
        _equipment.TurnEndEffects
            .ForEach(e => AllEffects.Apply(e, new EffectContext(_owner, new Single(_owner), Maybe<Card>.Missing(), 0)));
    }
}
