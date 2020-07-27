using UnityEngine;

public class Library : ScriptableObject
{
    [SerializeField] private Hero[] unlockedHeroes;
    [SerializeField] private CardType[] unlockedCards;

    public Hero[] UnlockedHeroes => unlockedHeroes;
    public CardType[] UnlockedCards => unlockedCards;
}
