using System.Linq;
using UnityEngine;

public class BattleV4TutorialSetup : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BaseHero trainee;
    
    public void PerformSetup()
    {
        party.Initialized(trainee);
        CurrentAcademyData.Write(s =>
        {
            s.TutorialData = new AcademyTutorialData
            {
                CompletedTutorialNames = s.TutorialData.CompletedTutorialNames
                    .Where(x => !x.StartsWith("BattleV4-"))
                    .ToArray()
            };
            return s;
        });
    }
}
