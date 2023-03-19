using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/Library")]
public class Library : ScriptableObject
{
    [SerializeField] private BaseHero featuredHero;
    [SerializeField] private AllHeroes allHeroes;
    [SerializeField] private AllCards allCards;
    [SerializeField] private AllEquipment allEquipment;
    [SerializeField] private AllLevelUpOptions allLevelUps;
    [SerializeField] private AllBosses allBosses;
    [SerializeField] private BaseHero noHero;
    [SerializeField] private BaseHero[] unlockedHeroes;
    [SerializeField] private Adventure[] unlockedAdventures;
    [SerializeField] private Difficulty[] unlockedDifficulties;

    public BaseHero NoHero => noHero;
    public Maybe<BaseHero> MaybeFeaturedHero => featuredHero != null ? featuredHero : Maybe<BaseHero>.Missing();
    public Boss[] AllBosses => allBosses.bosses.ToArray();
    public BaseHero[] UnlockedHeroes => unlockedHeroes.Where(h => !h.IsDisabled).ToArray();
    public Adventure[] UnlockedAdventures => unlockedAdventures;
    public Difficulty[] UnlockedDifficulties => unlockedDifficulties;
    public Difficulty DefaultDifficulty => UnlockedDifficulties.FirstOrDefault(x => x.id == 0) ?? UnlockedDifficulties.First();

    public BaseHero HeroById(int id) => allHeroes.GetHeroByIdOrDefault(id);
    public Maybe<CardTypeData> GetCardById(int id) => allCards.GetCardById(id);
    
    public Maybe<Equipment> GetEquipment(GameEquipmentData data) => allEquipment.GetFromSaveData(data);
    public Maybe<StaticHeroLevelUpOption> GetLevelUpPerkById(int id) => allLevelUps.GetLevelUpPerkById(id);

    public Maybe<Adventure> GetAdventureById(int adventureId) => unlockedAdventures.Where(a => a.id == adventureId).FirstAsMaybe();

    public Difficulty GetDifficulty(int id) => UnlockedDifficulties.FirstOrDefault(x => x.id == id) ?? DefaultDifficulty;
}
