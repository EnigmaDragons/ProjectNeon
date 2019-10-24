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
        SetupHero(hero1, heroes[0]);
        SetupHero(hero2, heroes[1]);
        SetupHero(hero3, heroes[2]);
        partyArea.WithUiPositions(new[] { hero1.transform, hero2.transform, hero3.transform });
        onPartySetupFinished.Publish();
    }

    private void SetupHero(GameObject heroOrigin, Hero hero)
    {
        var hasBody = !hero.Body.name.Equals("BodyPlaceholder");
        if (hasBody)
        {
            Instantiate(hero.Body, heroOrigin.transform.position, Quaternion.identity, heroOrigin.transform);
        }
        else
        {
            heroOrigin.GetComponent<SpriteRenderer>().sprite = hero.Bust;
        }
    }
}
