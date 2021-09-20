using UnityEngine;

public class PartySetupTester : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private BaseHero[] heroes;
    [SerializeField] private Adventure adventure;
    
    void Awake()
    {
        party.Initialized(heroes[0], heroes[1], heroes[2]);
        progress.Init(adventure, 0);
    }
}
