using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Library")]
public class Library : ScriptableObject
{
    [SerializeField] private BaseHero[] unlockedHeroes;
    [SerializeField] public AllCards allCards;
    [SerializeField] private Adventure[] unlockedAdventures;
    [SerializeField] private BaseHero noHero;

    public BaseHero[] UnlockedHeroes => unlockedHeroes;
    public Adventure[] UnlockedAdventures => unlockedAdventures;

    public BaseHero HeroById(int id) => unlockedHeroes.Where(h => h.Id == id).FirstAsMaybe().Select(h => h, () => noHero);
    public Maybe<CardTypeData> GetCardById(int id) => allCards.GetCardById(id);

    public Maybe<Adventure> GetAdventureById(int adventureId) => unlockedAdventures.Where(a => a.id == adventureId).FirstAsMaybe();
}
