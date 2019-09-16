using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{

    [SerializeField] private PartyArea partyArea;

    void Start()
    {
        Instantiate(partyArea.party.characterOne.Bust, new Vector3(150, 10, transform.position.z), Quaternion.identity, gameObject.transform);
        Instantiate(partyArea.party.characterTwo.Bust, new Vector3(-180, -25, transform.position.z), Quaternion.identity, gameObject.transform);
        Instantiate(partyArea.party.characterThree.Bust, new Vector3(0, -65, transform.position.z), Quaternion.identity, gameObject.transform);
    }
}
