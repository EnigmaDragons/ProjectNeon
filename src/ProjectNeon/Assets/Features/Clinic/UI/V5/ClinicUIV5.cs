using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class ClinicUIV5 : OnMessage<UpdateClinic, RefreshShop>, ILocalizeTerms
{
    [SerializeField] private GameObject patientParent;
    [SerializeField] private ClinicPatientUIV5 patientPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private CorpClinicProvider clinics;
    [SerializeField] private Localize serviceTitleLocalize;
    [SerializeField] private GameObject servicesParent;
    [SerializeField] private ClinicServiceButtonV5 serviceButtonPrototype;
    [SerializeField] private CorpUiBase[] corpUi = new CorpUiBase[0];
    [SerializeField] private Button doneButton;

    private ClinicServiceProvider _serviceProvider;
    private ClinicServiceButtonV5[] _serviceButtons;
    private int numInitialVouchers;
    private List<string> selectedServices = new List<string>();
    private List<string> unselectedServices = new List<string>();
    
    protected override void Execute(UpdateClinic msg) => UpdateServices();
    protected override void Execute(RefreshShop msg)
    {
        _serviceProvider = clinics.GetServices(clinic.Corp);
        UpdateServices();
    }

    protected override void AfterDisable()
    {
        AllMetrics.PublishClinicServiceSelection(numInitialVouchers, party.ClinicVouchers, selectedServices.ToArray(), unselectedServices.ToArray());
        Message.Publish(new AutoSaveRequested());
    }

    private void InitMetricData()
    {
        numInitialVouchers = party.ClinicVouchers;
        selectedServices = new List<string>();
        unselectedServices = new List<string>();
    }

    private void RecordSelected(string serviceName)
    {
        unselectedServices.Remove(serviceName);
        selectedServices.Add(serviceName);
    }

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
        InitMetricData();
        if (clinic.Corp != null && corpUi != null)
            corpUi.ForEach(c => c.Init(clinic.Corp));
        doneButton.interactable = !_serviceProvider.RequiresSelection() && (!clinic.IsTutorial || party.ClinicVouchers == 0 || party.Heroes.All(x => x.Health.MissingHp == 0));
        var options = _serviceProvider.GetOptions().Select(o => o.WithAdditionalAction(() => RecordSelected(o.MetricDescription))).ToArray();
        serviceTitleLocalize.SetFinalText(options.Length > 0 
            ? $"{_serviceProvider.GetTitleTerm().ToLocalized()}{(_serviceProvider.RequiresSelection() ? $" ({"Clinics/SelectionRequired".ToLocalized()})" : "")}"
            : "");
        for (var i = 0; i < _serviceButtons.Length; i++)
        {
            _serviceButtons[i].Init(options[i], party);
            unselectedServices.Add(options[i].MetricDescription);
        }
    }

    public string[] GetLocalizeTerms()
        => new [] { "Clinics/SelectionRequired" };
}
