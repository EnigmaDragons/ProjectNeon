using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyArea : ScriptableObject
{
    [SerializeField]
    public Party party;

    public PartyArea(Party party)
    {
        this.party = party;
    }
}
