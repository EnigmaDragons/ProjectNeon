using System.Linq;
using UnityEngine;

public class GetUserSelectedHeroOnStart : MonoBehaviour
{
    [SerializeField] private Library library;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Party currentParty;
    [SerializeField] private bool triggerOnStart = true;

    private void Start()
    {
        if (triggerOnStart)
            Trigger();
    }

    public void Trigger()
    {
        party.Initialized();
        
        var allOptions = library.UnlockedHeroes.ToList();
        currentParty.Heroes.ForEach(h => allOptions.Remove(h));

        var currentArchs = currentParty.Heroes.SelectMany(h => h.Archetypes).ToHashSet();
        var preferredSelection = allOptions.Where(h => !h.Archetypes.Any(a => currentArchs.Contains(a))).ToList();
        var optimizedSelection = preferredSelection.Count() >= 3 ? preferredSelection : allOptions;

        var randomThree = optimizedSelection.Shuffled().Take(3).ToArray();
        var prompt = currentParty.Heroes.Length == 0 ? "Choose Your Mission Squad Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, randomThree, h =>
        {
            Message.Publish(new AddHeroToPartyRequested(h));
            Async.ExecuteAfterDelay(0.5f, () => Message.Publish(new ToggleNamedTarget("HeroSelectionView")));
        })); 
    }
}
