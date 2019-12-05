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
			
			//FakeMirror
			GameObject clone = Instantiate(hero.Body, heroOrigin.transform.position, Quaternion.identity, heroOrigin.transform); //new clone from Target with new position
			clone.transform.localScale = new Vector3(1f,-1f,1f); //set clone obj new Scale flip y
			clone.transform.position  += new Vector3(0,-1.55f,0); //set clone obj new position y
			clone.GetComponentInChildren<SpriteRenderer>().color = new Color(1f,1f,1f, 0.3f); //set clone obj alpha opacity
			clone.GetComponentInChildren<Renderer>().sortingOrder = 1;//set clone obj Order in Layer
			
        }
        else
        {
            heroOrigin.GetComponent<SpriteRenderer>().sprite = hero.Bust;
        }
    }
}
