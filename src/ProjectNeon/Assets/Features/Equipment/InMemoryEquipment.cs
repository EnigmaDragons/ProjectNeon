public class InMemoryEquipment : Equipment
{
    public int Id { get; set; } = -1;
    public string Name { set; get; }
    public string Description { set; get; }
    public int Price { set; get; }
    public Rarity Rarity { set; get; }
    public string[] Archetypes { set; get; } = new string[0];
    public EquipmentSlot Slot { set; get; }
    public EquipmentStatModifier[] Modifiers { get; set; } = new EquipmentStatModifier[0];
    public IResourceType[] ResourceModifiers { get; set; } = new IResourceType[0];
    public EffectData[] TurnStartEffects { get; set; } = new EffectData[0];
    public EffectData[] TurnEndEffects { get; set; } = new EffectData[0];
    public EffectData[] BattleStartEffects { get; set; } = new EffectData[0];

    public Equipment Initialized(string archetype)
    {
        Archetypes = new[] {archetype};
        return this;
    }
}
