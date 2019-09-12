using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Party : ScriptableObject
{
    private Character characterOne = new NoCharacter();
    private Character characterTwo;
    private Character characterThree;

    public Party( Character one, Character two, Character three)
    {
        this.characterOne = one;
        this.characterTwo = two;
        this.characterThree = three;
    }

}
