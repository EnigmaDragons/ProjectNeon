using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupParty : MonoBehaviour
{

    [SerializeField]
    private Party party;

    private PartyArea partyArea;

    // Start is called before the first frame update
    void Start()
    {
        this.partyArea = new PartyArea(party);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
