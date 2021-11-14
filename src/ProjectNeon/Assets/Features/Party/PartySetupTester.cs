using Features.GameProgression;
using UnityEngine;

public class PartySetupTester : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private BaseHero[] heroes;
    [SerializeField] private Adventure adventure;
    
    void Awake()
    {
        party.Initialized(heroes[0], heroes[1], heroes[2]);
        progress.AdventureProgress.Init(adventure, 0);
    }
}
