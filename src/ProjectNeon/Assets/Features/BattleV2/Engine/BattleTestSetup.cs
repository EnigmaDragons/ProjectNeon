
using System.Collections.Generic;
using UnityEngine;

public class BattleTestSetup : MonoBehaviour
{
    [SerializeField] private BattleSetupV2 setup;
    
    [Header("Party")] 
    [SerializeField] private Hero hero1;
    [SerializeField] private Hero hero2;
    [SerializeField] private Hero hero3;

    [Header("BattleField")] 
    [SerializeField] private GameObject battlefield;

    [Header("Enemies")] 
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private EncounterBuilder encounterBuilder;

    public void UseCustomParty() => setup.InitParty(hero1, hero2, hero3);
    public void UseCustomBattlefield() => setup.InitBattleField(battlefield);
    public void UseFixedEncounter() => setup.InitEncounter(enemies);
    public void UseCustomEncounterSet() => setup.InitEncounterBuilder(encounterBuilder);
}
