using UnityEngine;

public class Library : ScriptableObject
{
    [SerializeField] private Hero[] unlockedHeroes;
    [SerializeField] private Card[] unlockedCards;

    public Hero[] UnlockedHeroes => unlockedHeroes;
    public Card[] UnlockedCards => unlockedCards;
}
