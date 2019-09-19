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

    [SerializeField] private IntVariable totalPowerLevel;

}
