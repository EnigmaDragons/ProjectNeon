using UnityEngine;
using UnityEngine.UI;

public class ClinicUIController : OnMessage<ToggleClinic>, ILocalizeTerms
{
    [SerializeField] private GameObject clinicUi;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private AllCorps corps;

    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Button doneButton;
    [SerializeField] private EventPublisher eventPublisher;

    private void Awake()
    {
        doneButton.onClick.AddListener(() =>
        {
            var shouldHealInjury = party.ClinicVouchers > 0 && party.TotalNumInjuries > 0;
            var shouldGetSomeHealing = party.ClinicVouchers > 0 && party.MissingHpFactor > 0.33;
            
            if (shouldHealInjury)
                Message.Publish(new ShowLocalizedDialog
                {
                    UseDarken = true,
                    PromptTerm = DialogTerms.LeaveClinicWithInjury,
                    PrimaryButtonTerm = DialogTerms.OptionLeave,
                    PrimaryAction = LeaveClinic,
                    SecondaryButtonTerm = DialogTerms.OptionStay
                });
            else if (shouldGetSomeHealing)
                Message.Publish(new ShowLocalizedDialog
                {
                    UseDarken = true,
                    PromptTerm = DialogTerms.LeaveClinicWithWounds,
                    PrimaryButtonTerm = DialogTerms.OptionLeave,
                    PrimaryAction = LeaveClinic,
                    SecondaryButtonTerm = DialogTerms.OptionStay
                });
            else
                LeaveClinic();    
        });
    }

    private void LeaveClinic()
    {
        eventPublisher.SaveGame();
        eventPublisher.FinishNode();
        eventPublisher.ToggleClinic();
    }
    
    protected override void Execute(ToggleClinic msg)
    {
        clinic.Corp = corps.GetCorpByNameOrNone(msg.CorpName);
        clinic.IsTutorial = msg.IsTutorial;
        clinicUi.SetActive(!clinicUi.activeSelf);
        if (msg.IsTutorial)
            Message.Publish(new SetSuperFocusHealControl(true));
    }

    public string[] GetLocalizeTerms() => new[]
    {
        DialogTerms.OptionLeave,
        DialogTerms.OptionStay,
        DialogTerms.LeaveClinicWithInjury,
        DialogTerms.LeaveClinicWithWounds
    };
}
