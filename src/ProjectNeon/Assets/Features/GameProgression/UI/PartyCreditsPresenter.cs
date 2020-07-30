using TMPro;
using UnityEngine;

public class PartyCreditsPresenter : OnMessage<PartyStateChanged>
{
    [SerializeField] private Party party;
    [SerializeField] private TextMeshProUGUI label;

    void Awake() => label.text = party.Credits.ToString();
    
    protected override void Execute(PartyStateChanged msg) 
        => label.text = party.Credits.ToString();
}
