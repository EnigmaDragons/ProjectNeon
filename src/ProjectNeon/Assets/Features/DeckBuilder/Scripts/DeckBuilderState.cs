﻿using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/DeckBuilder")]
public class DeckBuilderState : ScriptableObject
{
    private HeroesDeck _selectedHeroesDeck;

    public List<HeroesDeck> HeroesDecks { get; set; }
    public bool ShowFormulas { get; set; }
    public Rarity[] ShowRarities { get; set; }
    public string[] ShowArchetypes { get; set; }
    public EquipmentSlot[] ShowEquipmentSlots { get; set; }

    public HeroCharacter SelectedHeroCharacter => SelectedHeroesDeck.Hero.Character;
    public Hero SelectedHero => SelectedHeroesDeck.Hero;
    public HeroesDeck SelectedHeroesDeck
    {
        get => _selectedHeroesDeck;
        set
        {
            _selectedHeroesDeck = value;
            Message.Publish(new DeckBuilderHeroSelected(_selectedHeroesDeck));
        }
    }
    
    public Action OnDeckbuilderClosedAction { get; set; }
}
