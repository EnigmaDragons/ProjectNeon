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

    public ClinicCostCalculator GetCostCalculator(Corp corp)
    {
        if (healthPercentCorp.Contains(corp))
            return new HealthDependantClinicCostCalculator();
        if (credDependentCorp.Contains(corp))
            return new CredDependentClinicCostCalculator(party);
        return new ClinicCostCalculatorV1();
    }

    public ClinicServiceProvider GetServices(Corp corp)
    {
        if (procedureCorps.Contains(corp))
            return new ImplantClinicServiceProvider(party);
        if (blessingCorps.Contains(corp))
            return new BlessingClinicServiceProvider(party, blessings);
        return new NoClinicServiceProvider();
    }

    public Maybe<BlessingData> GetBlessingByName(string blessingName)
        => blessings.FirstOrMaybe(b => b.Name.Equals(blessingName));
}