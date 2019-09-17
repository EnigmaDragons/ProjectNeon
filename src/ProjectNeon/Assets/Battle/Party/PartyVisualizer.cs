using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{

    [SerializeField] private PartyArea partyArea;
    [SerializeField] private GameObject _character1;
    [SerializeField] private GameObject _character2;
    [SerializeField] private GameObject character3;


    void Start()
    {
        _character1.GetComponent<SpriteRenderer>().sprite = partyArea.party.characterOne.Bust;
        _character2.GetComponent<SpriteRenderer>().sprite = partyArea.party.characterTwo.Bust;
        character3.GetComponent<SpriteRenderer>().sprite = partyArea.party.characterThree.Bust;
    }
}
