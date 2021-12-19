using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class BattlefieldSet : ScriptableObject
{
    [SerializeField] private GameObject[] battlefields;
    [SerializeField] private GameObject lastUsed;

    public GameObject GetNext()
    {
        var battlefieldOptions = battlefields.ToList();
        if (lastUsed != null)
            battlefieldOptions.Remove(lastUsed);
        
        var selected = battlefieldOptions.Random();
        lastUsed = selected;
        return selected;
    }
}
