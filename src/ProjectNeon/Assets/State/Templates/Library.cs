using UnityEngine;

public class Library : ScriptableObject
{
    // @todo #1:10min Update Squad Selection to use Unlocked Characters as it's available character pool

    [SerializeField] private Character[] unlockedCharacters;
    [SerializeField] private Card[] unlockedCards;
}
