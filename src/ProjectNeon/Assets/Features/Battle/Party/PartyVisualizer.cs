using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{
    [SerializeField] private Party party;
    [SerializeField] private GameObject hero1;
    [SerializeField] private GameObject hero2;
    [SerializeField] private GameObject hero3;
    [SerializeField] private GameEvent onPartySetupFinished;

    // @todo #125:15min Dynamically create Heroes from a Prototype, instead of fixed slots
    
    void Start()
    {
        hero1.GetComponent<SpriteRenderer>().sprite = party.heroOne.Bust;
        hero2.GetComponent<SpriteRenderer>().sprite = party.heroTwo.Bust;
        hero3.GetComponent<SpriteRenderer>().sprite = party.heroThree.Bust;
        partyArea.WithUiPositions(new[] { character1.transform, character2.transform, character3.transform });
        onPartySetupFinished.Publish();
    }
}
