using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CorpClinicProvider")]
public class CorpClinicProvider : ScriptableObject
{
    [SerializeField] private StaticCorp[] healthPercentCorp;
    [SerializeField] private StaticCorp[] credDependentCorp;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private StaticCorp[] procedureCorps;
    [SerializeField] private StaticCorp[] blessingCorps;
    [SerializeField] private BlessingData[] blessings;
    [SerializeField] private BlessingData[] blessingsV4;
    [SerializeField] private ClinicState clinicState;
    [SerializeField] private int commonChances;
    [SerializeField] private int uncommonChances;
    [SerializeField] private int rareChances;
    [SerializeField] private int epicChances;

    private const int _tutorialAdventureId = 10;
    
    public ClinicCostCalculator GetCostCalculator(Corp corp)
    {
        if (CurrentGameData.Data.AdventureProgress.Type == GameAdventureProgressType.V4)
            return new StaticClinicCostCalculator(1);
        if (healthPercentCorp.Contains(corp))
            return new HealthDependantClinicCostCalculator(party, corp.Name);
        if (credDependentCorp.Contains(corp))
            return new CredDependentClinicCostCalculator(party, corp.Name);
        return new ClinicCostCalculatorV1(party, corp.Name);
    }

    public ClinicServiceProvider GetServices(Corp corp)
    {
        var adv = CurrentGameData.Data.AdventureProgress;
        var gameType = adv.Type;
        var rng = new DeterministicRng(adv.RngSeed);
        var rarityChances = new List<Rarity>();
        for (int i = 0; i < commonChances; i++)
            rarityChances.Add(Rarity.Common);
        for (int i = 0; i < uncommonChances; i++)
            rarityChances.Add(Rarity.Uncommon);
        for (int i = 0; i < rareChances; i++)
            rarityChances.Add(Rarity.Rare);
        for (int i = 0; i < epicChances; i++)
            rarityChances.Add(Rarity.Epic);
        if (adv.AdventureId == _tutorialAdventureId && gameType == GameAdventureProgressType.V5 && procedureCorps.Contains(corp))
            return new TutorialImplantClinicServiceProviderV5(party, adventure.Adventure.NumOfImplantOptions, rng);
        if (gameType == GameAdventureProgressType.V5 && procedureCorps.Contains(corp))
            return new ImplantClinicServiceProviderV4(party, adventure.Adventure.NumOfImplantOptions, rng, clinicState.IsTutorial, rarityChances.ToArray());
        if (gameType == GameAdventureProgressType.V5 && blessingCorps.Contains(corp))
            return new BlessingClinicServiceProviderV4(party, blessingsV4, rng);
        if (gameType == GameAdventureProgressType.V4 && procedureCorps.Contains(corp))
            return new ImplantClinicServiceProviderV4(party, adventure.Adventure.NumOfImplantOptions, rng, clinicState.IsTutorial, rarityChances.ToArray());
        if (gameType == GameAdventureProgressType.V4 && blessingCorps.Contains(corp))
            return new BlessingClinicServiceProviderV4(party, blessingsV4, rng);
        if (procedureCorps.Contains(corp))
            return new ImplantClinicServiceProvider(party);
        if (blessingCorps.Contains(corp))
            return new BlessingClinicServiceProvider(party, blessings);
        return new NoClinicServiceProvider();
    }

    public Maybe<BlessingData> GetBlessingByName(string blessingName)
        => CurrentGameData.Data.AdventureProgress.Type == GameAdventureProgressType.V4
            ?  blessingsV4.FirstOrMaybe(b => b.Name.Equals(blessingName))
            : blessings.FirstOrMaybe(b => b.Name.Equals(blessingName));
}