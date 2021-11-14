using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CorpClinicProvider")]
public class CorpClinicProvider : ScriptableObject
{
    [SerializeField] private StaticCorp[] healthPercentCorp;
    [SerializeField] private StaticCorp[] credDependentCorp;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private StaticCorp[] procedureCorps;
    [SerializeField] private StaticCorp[] blessingCorps;
    [SerializeField] private BlessingData[] blessings;
    [SerializeField] private BlessingData[] blessingsV4;

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
        if (CurrentGameData.Data.AdventureProgress.Type == GameAdventureProgressType.V4 && procedureCorps.Contains(corp))
            return new ImplantClinicServiceProviderV4(party);
        if (CurrentGameData.Data.AdventureProgress.Type == GameAdventureProgressType.V4 && blessingCorps.Contains(corp))
            return new BlessingClinicServiceProviderV4(party, blessingsV4);
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