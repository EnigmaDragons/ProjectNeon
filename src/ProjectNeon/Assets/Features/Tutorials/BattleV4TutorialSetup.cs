using UnityEngine;

public class BattleV4TutorialSetup : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BaseHero trainee;
    
    public void PerformSetup()
    {
        party.Initialized(trainee);
    }
}
