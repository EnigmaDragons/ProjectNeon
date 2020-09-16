using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/PartiallyGenerated")]
public sealed class PartiallyGeneratedEquipment : ScriptableObject, Equipment
{    
    [SerializeField] private string description;
    [SerializeField] private Rarity rarity;
    [SerializeField] private EquipmentSlot slot;
    [SerializeField] private int cost;
    [SerializeField] private CharacterClass[] canUseClasses = new CharacterClass[1];
    [SerializeField] private EquipmentStatModifier[] modifiers = new EquipmentStatModifier[1];
    [SerializeField] private ResourceTypeModifications[] resourceModifiers = new ResourceTypeModifications[0];
    [SerializeField] private EffectData[] turnStartEffects = new EffectData[0];
    [SerializeField] private EffectData[] turnEndEffects = new EffectData[0];
    [SerializeField] private EffectData[] battleStartEffects = new EffectData[0];
    [SerializeField] private int numSlots = 0;
    [SerializeField] private EquipmentStatModifier[] statModifierSlots = new EquipmentStatModifier[0];

    private EquipmentStatModifier[] _modifiers;
    
    public string Name => EquipmentGenerator.NameFor(slot, rarity);
    public int Price => cost;
    public Rarity Rarity => rarity;
    public string[] Classes => canUseClasses.Select(c => c.Name).ToArray();
    public EquipmentSlot Slot => slot;

    public IResourceType[] ResourceModifiers => resourceModifiers.Cast<IResourceType>().ToArray();
    public EffectData[] TurnStartEffects => turnStartEffects;
    public EffectData[] TurnEndEffects => turnEndEffects;
    public EffectData[] BattleStartEffects => battleStartEffects;

    public string Description
    {
        get
        {
            InitIfNeeded();
            var desc = description;
            foreach (var m in _modifiers) 
                desc += m.Describe();
            return desc;
        }
    }

    public EquipmentStatModifier[] Modifiers
    {
        get { 
            InitIfNeeded();
            return modifiers;
        }
    }
    
    private void InitIfNeeded()
    {
        if (modifiers != null)
            return;
        
        _modifiers = modifiers.Concat(Enumerable.Range(0, numSlots).Select(_ => statModifierSlots.Random())).ToArray();
    }
}
