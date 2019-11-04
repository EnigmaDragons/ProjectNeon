using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderState : ScriptableObject
{
    [SerializeField] private GameEvent heroSelected;

    private HeroesDeck _selectedHeroesDeck;

    public List<HeroesDeck> HeroesDecks { get; set; }

    public HeroesDeck SelectedHeroesDeck
    {
        get => _selectedHeroesDeck;
        set
        {
            _selectedHeroesDeck = value;
            heroSelected.Publish();
        }
    }
}
