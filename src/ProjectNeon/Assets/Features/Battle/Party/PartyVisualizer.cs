using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private GameObject hero1;
    [SerializeField] private GameObject hero2;
    [SerializeField] private GameObject hero3;
    [SerializeField] private GameEvent onPartySetupFinished;

    // @todo #125:15min Dynamically create Heroes from a Prototype, instead of fixed slots
    
    void Start()
    {
        var party = partyArea.Party;
        var heroes = party.Heroes;
        hero1.GetComponent<SpriteRenderer>().sprite = heroes[0].Bust;
        hero2.GetComponent<SpriteRenderer>().sprite = heroes[1].Bust;
        hero3.GetComponent<SpriteRenderer>().sprite = heroes[2].Bust;
        partyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        onPartySetupFinished.Publish();
    }
}
