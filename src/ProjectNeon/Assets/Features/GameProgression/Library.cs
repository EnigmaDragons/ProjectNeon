﻿using UnityEngine;

public class Library : ScriptableObject
{
    [SerializeField] private BaseHero[] unlockedHeroes;
    [SerializeField] private CardType[] unlockedCards;

    public BaseHero[] UnlockedHeroes => unlockedHeroes;
    public CardType[] UnlockedCards => unlockedCards;
}
