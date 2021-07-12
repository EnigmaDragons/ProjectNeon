using TMPro;
using UnityEngine;

public class ClinicUI : OnMessage<UpdateClinic>
{
    [SerializeField] private GameObject patientParent;
    [SerializeField] private ClinicPatientUI patientPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private CorpClinicProvider clinics;
    [SerializeField] private TextMeshProUGUI serviceTitle;
    [SerializeField] private GameObject servicesParent;
    [SerializeField] private ClinicServiceButton serviceButtonPrototype;
    [SerializeField] private CorpUiBase[] corpUi;

    private ClinicServiceProvider _serviceProvider;
    
    protected override void Execute(UpdateClinic msg) => UpdateServices();
    protected override void AfterDisable() => Message.Publish(new AutoSaveRequested());
    protected override void AfterEnable()
    {
        patientParent.DestroyAllChildren();
        var costCalculator = clinics.GetCostCalculator(clinic.Corp);
        party.Heroes.ForEach(h => Instantiate(patientPrototype, patientParent.transform).Initialized(h, costCalculator));
        _serviceProvider = clinics.GetServices(clinic.Corp);
        UpdateServices();
    }

    private void UpdateServices()
    {
        corpUi.ForEach(c => c.Init(clinic.Corp));
        serviceTitle.text = _serviceProvider.GetTitle();
        servicesParent.DestroyAllChildren();
        _serviceProvider.GetOptions().ForEach(x => Instantiate(serviceButtonPrototype, servicesParent.transform).Init(x, party));
    }
}
