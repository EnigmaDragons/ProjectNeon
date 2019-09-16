using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PartyArea : ScriptableObject
{

    public Party party;

    public PartyArea(Party party)
    {
        this.party = party;
    }
}
