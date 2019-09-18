using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{
    [SerializeField] private Party party;
    [SerializeField] private GameObject character1;
    [SerializeField] private GameObject character2;
    [SerializeField] private GameObject character3;
    [SerializeField] private GameEvent onPartySetupFinished;

    // @todo: #125:15min Dynamically create Characters from a Prototype, instead of fixed slots
    
    void Start()
    {
        character1.GetComponent<SpriteRenderer>().sprite = party.characterOne.Bust;
        character2.GetComponent<SpriteRenderer>().sprite = party.characterTwo.Bust;
        character3.GetComponent<SpriteRenderer>().sprite = party.characterThree.Bust;
        onPartySetupFinished.Publish();
    }
}
