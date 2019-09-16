using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyVisualizer : MonoBehaviour
{

    [SerializeField] private PartyArea partyArea;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void UpdateParty()
    {
        partyArea.party.characterOne;
        partyArea.party.characterTwo;
        partyArea.party.characterThree;


        for (var i = 0; i < ; i++)
        {
            var c = Instantiate(cardPrototype, new Vector3(minX + spaceBetweenCards * i, transform.position.y, transform.position.z), Quaternion.identity, gameObject.transform);
            c.Set(cards[i]);
        }
    }
}
