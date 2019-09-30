using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private GameObject character1;
    [SerializeField] private GameObject character2;
    [SerializeField] private GameObject character3;
    [SerializeField] private GameEvent onPartySetupFinished;

    // @todo #125:15min Dynamically create Characters from a Prototype, instead of fixed slots
    
    void Start()
    {
        var party = partyArea.Party;
        character1.GetComponent<SpriteRenderer>().sprite = party.characterOne.Bust;
        character2.GetComponent<SpriteRenderer>().sprite = party.characterTwo.Bust;
        character3.GetComponent<SpriteRenderer>().sprite = party.characterThree.Bust;
        partyArea.WithUiPositions(new[] {character1.transform, character2.transform, character3.transform});
        onPartySetupFinished.Publish();
    }
}
