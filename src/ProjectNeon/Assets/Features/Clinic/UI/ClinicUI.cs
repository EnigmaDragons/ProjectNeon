using System.Linq;
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
    private ClinicServiceButton[] _serviceButtons;

    private void Awake()
    {
        V4ClinicHackOnLeave();
    }
    
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
        V4ClinicHackOnEnter();
        patientParent.DestroyAllChildren();
        var costCalculator = clinics.GetCostCalculator(clinic.Corp);
        party.Heroes.ForEach(h => Instantiate(patientPrototype, patientParent.transform).Initialized(h, costCalculator));
        _serviceProvider = clinics.GetServices(clinic.Corp);
        servicesParent.DestroyAllChildren();
        _serviceButtons = _serviceProvider.GetOptions().Select(x => Instantiate(serviceButtonPrototype, servicesParent.transform)).ToArray();
        UpdateServices();
    }
    
    private void UpdateServices()
    {
        if (clinic.Corp != null && corpUi != null)
            corpUi.ForEach(c => c.Init(clinic.Corp));
        doneButton.interactable = !_serviceProvider.RequiresSelection();
        serviceTitle.text = $"{_serviceProvider.GetTitleTerm()}{(_serviceProvider.RequiresSelection() ? " (Selection Required To Leave)" : string.Empty)}";
        var options = _serviceProvider.GetOptions();
        for (var i = 0; i < _serviceButtons.Length; i++)
        {
            _serviceButtons[i].Init(options[i], party);
        }
    }
    
    private void V4ClinicHackOnEnter()
    {
        if (CurrentGameData.Data.AdventureProgress.Type == GameAdventureProgressType.V4)
        {
            party.UpdateCreditsBy(-9999, false);
            party.UpdateCreditsBy(party.Heroes.Length + 1);
        }
    }
    
    private void V4ClinicHackOnLeave()
    {
        if (CurrentGameData.Data.AdventureProgress.Type == GameAdventureProgressType.V4)
            doneButton.onClick.AddListener(() => party.UpdateCreditsBy(-9999, false));
    }
}
