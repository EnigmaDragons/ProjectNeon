using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Library")]
public class Library : ScriptableObject
{
    [SerializeField] private BaseHero[] unlockedHeroes;
    [SerializeField] private CardType[] unlockedCards;
    [SerializeField] private Adventure[] unlockedAdventures;
    [SerializeField] private BaseHero noHero;
    
    public BaseHero[] UnlockedHeroes => unlockedHeroes;
    public CardType[] UnlockedCards => unlockedCards;
    public Adventure[] UnlockedAdventures => unlockedAdventures;

    public BaseHero HeroById(int id) => unlockedHeroes.Where(h => h.Id == id).FirstAsMaybe().Select(h => h, () => noHero);
}
