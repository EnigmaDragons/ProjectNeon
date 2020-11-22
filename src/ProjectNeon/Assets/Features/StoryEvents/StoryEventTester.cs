
using UnityEngine;

public class StoryEventTester : MonoBehaviour
{
    [SerializeField] private StoryEvent storyEvent;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BaseHero hero1;
    [SerializeField] private BaseHero hero2;
    [SerializeField] private BaseHero hero3;

    private void Start()
    {
        party.Initialized(hero1, hero2, hero3);
        Message.Publish(new BeginStoryEvent(storyEvent));
    }
}
