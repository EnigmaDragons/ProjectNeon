using TMPro;
using UnityEngine;

public class PartyCreditsPresenter : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI label;

    protected override void AfterEnable() => Render();
    
    protected override void Execute(PartyAdventureStateChanged msg) => Render();

    private void Render() => label.text = party.Credits.ToString() + 0;
}
