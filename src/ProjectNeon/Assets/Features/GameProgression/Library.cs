using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Library")]
public class Library : ScriptableObject
{
    [SerializeField] private BaseHero[] unlockedHeroes;
    [SerializeField] private AllHeroes allHeroes;
    [SerializeField] private AllCards allCards;
    [SerializeField] private AllEquipment allEquipment;
    [SerializeField] private AllLevelUpOptions allLevelUps;
    [SerializeField] private Adventure[] unlockedAdventures;
    [SerializeField] private BaseHero noHero;

    public BaseHero NoHero => noHero;
    public BaseHero[] UnlockedHeroes => unlockedHeroes;
    public Adventure[] UnlockedAdventures => unlockedAdventures;

    public BaseHero HeroById(int id) => allHeroes.GetHeroByIdOrDefault(id);
    public Maybe<CardTypeData> GetCardById(int id) => allCards.GetCardById(id);
    
    public Maybe<Equipment> GetEquipment(GameEquipmentData data) => allEquipment.GetFromSaveData(data);
    public Maybe<HeroLevelUpOption> GetLevelUpPerkById(int id) => allLevelUps.GetLevelUpPerkById(id);

    public Maybe<Adventure> GetAdventureById(int adventureId) => unlockedAdventures.Where(a => a.id == adventureId).FirstAsMaybe();
}
