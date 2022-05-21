using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/DeckBuilder")]
public class DeckBuilderState : ScriptableObject
{
    [SerializeField] private IntReference deckSize;
    
    private HeroesDeck _selectedHeroesDeck;

    public bool AllHeroDecksAreValid => HeroesDecks != null && HeroesDecks.Any() && HeroesDecks.Where(h => h != null && h.Deck != null).All(h => h.Deck.Count == deckSize);
    public bool SelectedHeroDeckIsValid => SelectedHeroesDeck.Deck.Count == deckSize;

    public List<HeroesDeck> HeroesDecks { get; set; } = new List<HeroesDeck>();
    
    public bool ShowFormulas { get; set; } = false;
    public EquipmentSlot[] ShowEquipmentSlots { get; set; } = new EquipmentSlot[0];

    public HeroCharacter SelectedHeroCharacter => SelectedHeroesDeck.Hero.Character;
    public Hero SelectedHero => SelectedHeroesDeck.Hero;
    public HeroesDeck SelectedHeroesDeck
    {
        get => _selectedHeroesDeck;
        set
        {
            _selectedHeroesDeck = value;
            Message.Publish(new DeckBuilderHeroSelected(_selectedHeroesDeck));
            Message.Publish(new DeckBuilderStateUpdated());
        }
    }

    public Action OnDeckbuilderClosedAction { get; set; } = () => { };
}
