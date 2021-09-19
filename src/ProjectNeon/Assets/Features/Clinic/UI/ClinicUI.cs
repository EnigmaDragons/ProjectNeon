using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicUI : OnMessage<UpdateClinic, RefreshShop>
{
    [SerializeField] private GameObject patientParent;
    [SerializeField] private ClinicPatientUI patientPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private CorpClinicProvider clinics;
    [SerializeField] private TextMeshProUGUI serviceTitle;
    [SerializeField] private GameObject servicesParent;
    [SerializeField] private ClinicServiceButton serviceButtonPrototype;
    [SerializeField] private CorpUiBase[] corpUi = new CorpUiBase[0];
    [SerializeField] private Button doneButton;

    private ClinicServiceProvider _serviceProvider;
    
    protected override void Execute(UpdateClinic msg) => UpdateServices();
    protected override void Execute(RefreshShop msg)
    {
        _serviceProvider = clinics.GetServices(clinic.Corp);
        UpdateServices();
    }

    protected override void AfterDisable() => Message.Publish(new AutoSaveRequested());
    protected override void AfterEnable()
    {
        if (clinic.Corp == null)
            return;
        patientParent.DestroyAllChildren();
        var costCalculator = clinics.GetCostCalculator(clinic.Corp);
        party.Heroes.ForEach(h => Instantiate(patientPrototype, patientParent.transform).Initialized(h, costCalculator));
        _serviceProvider = clinics.GetServices(clinic.Corp);
        UpdateServices();
    }

    private void UpdateServices()
    {
        if (clinic.Corp != null && corpUi != null)
            corpUi.ForEach(c => c.Init(clinic.Corp));
        doneButton.interactable = !_serviceProvider.RequiresSelection();
        serviceTitle.text = $"{_serviceProvider.GetTitle()}{(_serviceProvider.RequiresSelection() ? " (Selection Required To Leave)" : "")}";
        servicesParent.DestroyAllChildren();
        _serviceProvider.GetOptions().ForEach(x => Instantiate(serviceButtonPrototype, servicesParent.transform).Init(x, party));
    }
}
