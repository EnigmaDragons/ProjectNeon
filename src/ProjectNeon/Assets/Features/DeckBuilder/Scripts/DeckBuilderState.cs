using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/DeckBuilder")]
public class DeckBuilderState : ScriptableObject
{
    private HeroesDeck _selectedHeroesDeck;

    public List<HeroesDeck> HeroesDecks { get; set; }

    public HeroesDeck SelectedHeroesDeck
    {
        get => _selectedHeroesDeck;
        set
        {
            _selectedHeroesDeck = value;
            Message.Publish(new DeckBuilderHeroSelected(_selectedHeroesDeck));
        }
    }
}
