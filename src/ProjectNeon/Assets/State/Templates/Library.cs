using UnityEngine;

public class Library : ScriptableObject
{
    [SerializeField] private Character[] unlockedCharacters;
    [SerializeField] private Card[] unlockedCards;

    public Character[] UnlockedCharacters => unlockedCharacters;    
}
