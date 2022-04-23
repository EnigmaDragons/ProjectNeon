using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class BattlefieldSet : ScriptableObject
{
    [SerializeField] private GameObject[] battlefields;
    [SerializeField] private int lastUsedId;

    public GameObject GetNext()
    {
        var battlefieldOptions = battlefields
            .Select((b, i) => (b, i))
            .Where(x => x.i != lastUsedId).ToList();
        
        (GameObject battlefield, int index) selected = battlefieldOptions.Random();
        lastUsedId = selected.index;
        return selected.battlefield;
    }
}
