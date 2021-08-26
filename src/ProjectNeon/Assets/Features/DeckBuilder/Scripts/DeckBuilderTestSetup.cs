using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckBuilderTestSetup : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BattleState battle;
    
    [Header("Party")] 
    [SerializeField] private BaseHero hero1;
    [SerializeField] private BaseHero hero2;
    [SerializeField] private BaseHero hero3;
    [SerializeField] private List<StaticEquipment> partyEquipment = new List<StaticEquipment>();
    [SerializeField] private List<StaticEquipment> hero1Equipment = new List<StaticEquipment>();
    [SerializeField] private List<StaticEquipment> hero2Equipment = new List<StaticEquipment>();
    [SerializeField] private List<StaticEquipment> hero3Equipment = new List<StaticEquipment>();
    
    [Header("Enemies")]
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private int stage;
    
    void Awake()
    {
        party.Initialized(hero1, hero2, hero3);
        InitPartyEquipment(hero1Equipment, hero2Equipment, hero3Equipment);
        SetupFixedEncounter();
    }
    
    public void InitPartyEquipment(IEnumerable<Equipment> h1, IEnumerable<Equipment> h2, IEnumerable<Equipment> h3)
    {
        partyEquipment.ForEach(e => party.Add(e));
        h1.ForEach(e => party.Add(e));
        h2.ForEach(e => party.Add(e));
        h3.ForEach(e => party.Add(e));
        h1.ForEach(e => InitEquipmentForHero(party.Heroes[0], e));
        h2.ForEach(e => InitEquipmentForHero(party.Heroes[1], e));
        h3.ForEach(e => InitEquipmentForHero(party.Heroes[2], e));
    }
    
    private void SetupFixedEncounter() => battle.SetNextEncounter(enemies.Select(x => x.ForStage(stage)));
    
    private void InitEquipmentForHero(Hero hero, Equipment equip)
    {
        if (equip.Slot == EquipmentSlot.Permanent)
            hero.ApplyPermanent(equip);
        else
            party.EquipTo(equip, hero);
    }
}
