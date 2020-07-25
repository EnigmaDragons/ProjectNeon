
using UnityEngine;

public class BattleTestSetup : MonoBehaviour
{
    [SerializeField] private BattleSetupV2 setup;
    
    [Header("Editor Battle Setup")] 
    [SerializeField] private Hero hero1;
    [SerializeField] private Hero hero2;
    [SerializeField] private Hero hero3;

    public void UseCustomParty() => setup.InitParty(hero1, hero2, hero3);
}
