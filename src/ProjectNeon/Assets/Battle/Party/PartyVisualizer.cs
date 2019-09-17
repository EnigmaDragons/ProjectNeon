using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        character1.GetComponent<SpriteRenderer>().sprite = partyArea.party.characterOne.Bust;
        character2.GetComponent<SpriteRenderer>().sprite = partyArea.party.characterTwo.Bust;
        character3.GetComponent<SpriteRenderer>().sprite = partyArea.party.characterThree.Bust;
    }
}
