using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicUIV5 : OnMessage<UpdateClinic, RefreshShop>
{
    [SerializeField] private GameObject patientParent;
    [SerializeField] private ClinicPatientUIV5 patientPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private CorpClinicProvider clinics;
    [SerializeField] private TextMeshProUGUI serviceTitle;
    [SerializeField] private GameObject servicesParent;
    [SerializeField] private ClinicServiceButtonV5 serviceButtonPrototype;
    [SerializeField] private CorpUiBase[] corpUi = new CorpUiBase[0];
    [SerializeField] private Button doneButton;

    private ClinicServiceProvider _serviceProvider;
    private ClinicServiceButtonV5[] _serviceButtons;
    
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
        party.Heroes.ForEach(h => Instantiate(patientPrototype, patientParent.transform).Initialized(h));
        _serviceProvider = clinics.GetServices(clinic.Corp);
        servicesParent.DestroyAllChildren();
        var options = _serviceProvider.GetOptions();
        Log.Info($"{_serviceProvider.GetType().Name} - NumOptions {options.Length}");
        _serviceButtons = options.Select(x => Instantiate(serviceButtonPrototype, servicesParent.transform)).ToArray();
        UpdateServices();
    }
    
    private void UpdateServices()
    {
        Log.Info("Clinic - Update Services");
        if (clinic.Corp != null && corpUi != null)
            corpUi.ForEach(c => c.Init(clinic.Corp));
        doneButton.interactable = !_serviceProvider.RequiresSelection() && (!clinic.IsTutorial || party.ClinicVouchers == 0 || party.Heroes.All(x => x.Health.MissingHp == 0));
        var options = _serviceProvider.GetOptions();
        serviceTitle.text = options.Length > 0 
            ? $"{_serviceProvider.GetTitle()}{(_serviceProvider.RequiresSelection() ? " (Selection Required To Leave)" : "")}"
            : "";
        for (var i = 0; i < _serviceButtons.Length; i++)
        {
            _serviceButtons[i].Init(options[i], party);
        }
    }
}
