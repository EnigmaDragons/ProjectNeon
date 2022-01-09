
using UnityEngine;

public class StoryEventTester : MonoBehaviour
{
    [SerializeField] private Adventure startAdventure;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private AdventureProgress2 adventureProgress2;
    [SerializeField] private StoryEvent2 storyEvent;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BaseHero hero1;
    [SerializeField] private BaseHero hero2;
    [SerializeField] private BaseHero hero3;

    private void Start()
    {
        currentAdventure.Adventure = startAdventure;
        adventureProgress2.InitIfNeeded();
        party.Initialized(hero1, hero2, hero3);
        Message.Publish(new BeginStoryEvent2(storyEvent));
    }
}
