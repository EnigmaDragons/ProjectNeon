using System.Linq;
using UnityEngine;

public class UiEmphasisStateHandler : OnMessage<PartyAdventureStateChanged, ShowDeckBuilder, TogglePartyDetails>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private UiEmphasisState emphasis;

    private int _numEquipment;
    private int _numCards;
    private int _numLevelUpPoints;

    private void Awake()
    {
        _numEquipment = party.Equipment.All.Count;
        _numCards = party.Cards.AllCards.Sum(i => i.Value);
        _numLevelUpPoints = party.Heroes.Sum(h => h.Levels.UnspentLevelUpPoints);
    }
    
    protected override void Execute(PartyAdventureStateChanged msg)
    {
        var newNumCards = party.Cards.AllCards.Sum(i => i.Value);
        if (newNumCards > _numCards)
            emphasis.Add("Decks");
        _numCards = newNumCards;
        
        var newNumEquipment = party.Equipment.All.Count;
        if (newNumEquipment > _numEquipment)
            emphasis.Add("Party");
        _numEquipment = newNumEquipment;

        var newNumLevelUpPoints = party.Heroes.Sum(h => h.Levels.UnspentLevelUpPoints);
        if (newNumLevelUpPoints > _numLevelUpPoints)
            emphasis.Add("Party");
        _numLevelUpPoints = newNumLevelUpPoints;
    }

    protected override void Execute(ShowDeckBuilder msg) => emphasis.Remove("Decks");
    protected override void Execute(TogglePartyDetails msg) => emphasis.Remove("Party");
}
