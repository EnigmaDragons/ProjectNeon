using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Party : ScriptableObject
{
    [SerializeField]
    public Character characterOne;

    [SerializeField]
    public Character characterTwo;

    [SerializeField]
    public Character characterThree;

}
