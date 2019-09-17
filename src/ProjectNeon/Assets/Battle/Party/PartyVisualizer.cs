using UnityEngine;
using UnityEngine.UI;

public class PartyVisualizer : MonoBehaviour
{

    [SerializeField] private PartyArea partyArea;
    [SerializeField] private GameObject character1;
    [SerializeField] private GameObject character2;
    [SerializeField] private GameObject character3;

    /**
     * @todo #28:15min Render player characters in battle screen.
     */

    void Start()
    {
        character1.GetComponent<Image>().sprite = partyArea.party.characterOne.Bust;
        character2.GetComponent<Image>().sprite = partyArea.party.characterTwo.Bust;
        character3.GetComponent<Image>().sprite = partyArea.party.characterThree.Bust;
    }
}
