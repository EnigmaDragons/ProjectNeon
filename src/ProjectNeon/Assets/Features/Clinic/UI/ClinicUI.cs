using TMPro;
using UnityEngine;

public class ClinicUI : OnMessage<RequestClinicHealService>
{
    [SerializeField] private GameObject patientParent;
    [SerializeField] private ClinicPatientUI patientPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextMeshProUGUI serviceCostLabel;
    [SerializeField] private int subsequentServiceCost = 20;
    
    protected override void AfterDisable() => Message.Publish(new AutoSaveRequested());
    protected override void AfterEnable()
    {
        patientParent.DestroyAllChildren();
        party.Heroes.ForEach(h => Instantiate(patientPrototype, patientParent.transform).Initialized(h));
        // The First Clinic Service is free
        UpdateRates(0);
    }

    protected override void Execute(RequestClinicHealService msg) => UpdateRates(subsequentServiceCost);

    private void UpdateRates(int amount)
    {
        serviceCostLabel.text = amount.ToString();
        Message.Publish(new UpdateClinicServiceRates(amount));
    }
}
