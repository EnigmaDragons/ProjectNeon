using UnityEngine;
using UnityEngine.UI;

public class ClinicUIController : OnMessage<ToggleClinic>
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
            
            if (shouldHealInjury || shouldGetSomeHealing)
                Message.Publish(new ShowTwoChoiceDialog
                {
                    UseDarken = true, 
                    Prompt = $"Are you sure you're ready to leave? You still have untreated {(shouldHealInjury ? "injuries" : "wounds")}!",
                    PrimaryButtonText = "Leave Anyway",
                    PrimaryAction = LeaveClinic,
                    SecondaryButtonText = "Stay"
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
    } 
}
