using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Party : ScriptableObject
{
    [SerializeField]
    private Character characterOne;

    [SerializeField]
    private Character characterTwo;

    [SerializeField]
    private Character characterThree;

}
