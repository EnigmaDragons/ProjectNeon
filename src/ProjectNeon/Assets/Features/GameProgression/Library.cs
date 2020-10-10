using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Library")]
public class Library : ScriptableObject
{
    [SerializeField] private BaseHero[] unlockedHeroes;
    [SerializeField] private CardType[] unlockedCards;
    [SerializeField] private Adventure[] unlockedAdventures;

    public BaseHero[] UnlockedHeroes => unlockedHeroes;
    public CardType[] UnlockedCards => unlockedCards;
    public Adventure[] UnlockedAdventures => unlockedAdventures;
}
