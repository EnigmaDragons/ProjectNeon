using TMPro;
using UnityEngine;

public class PartyCreditsPresenter : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextMeshProUGUI label;

    void Awake() => label.text = party.Credits.ToString();
    
    protected override void Execute(PartyAdventureStateChanged msg) 
        => label.text = party.Credits.ToString();
}
