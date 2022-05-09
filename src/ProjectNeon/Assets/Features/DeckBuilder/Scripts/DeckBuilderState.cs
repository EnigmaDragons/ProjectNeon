using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/DeckBuilder")]
public class DeckBuilderState : ScriptableObject
{
    [SerializeField] private IntReference deckSize;
    
    private HeroesDeck _selectedHeroesDeck;

    public bool AllHeroDecksAreValid => HeroesDecks.All(h => h.Deck.Count == deckSize);
    public bool SelectedHeroDeckIsValid => SelectedHeroesDeck.Deck.Count == deckSize;
    
    public List<HeroesDeck> HeroesDecks { get; set; }
    
    public bool ShowFormulas { get; set; } = false;
    public Rarity[] ShowRarities { get; set; } = {Rarity.Basic, Rarity.Common, Rarity.Rare, Rarity.Epic, Rarity.Starter};
    public string[] ShowArchetypes { get; set; }
    public EquipmentSlot[] ShowEquipmentSlots { get; set; } = new EquipmentSlot[0];

    public HeroCharacter SelectedHeroCharacter => SelectedHeroesDeck.Hero.Character;
    public Hero SelectedHero => SelectedHeroesDeck.Hero;
    public HeroesDeck SelectedHeroesDeck
    {
        get => _selectedHeroesDeck;
        set
        {
            _selectedHeroesDeck = value;
            ShowArchetypes = value.Hero.Archetypes.Concat("").ToArray();
            Message.Publish(new DeckBuilderHeroSelected(_selectedHeroesDeck));
            Message.Publish(new DeckBuilderFiltersChanged());
        }
    }

    public Action OnDeckbuilderClosedAction { get; set; }
}
