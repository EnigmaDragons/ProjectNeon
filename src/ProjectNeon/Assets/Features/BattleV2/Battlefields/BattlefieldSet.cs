using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class BattlefieldSet : ScriptableObject
{
    [SerializeField] private GameObject[] battlefields;
    [SerializeField] private int lastUsedId = -1;

    public GameObject GetNext()
    {
        var battlefieldOptions = battlefields
            .Select((b, i) => (b, i))
            .Where(x => x.i != lastUsedId).ToList();

        if (battlefieldOptions.None())
            battlefieldOptions = battlefields.Select((b, i) => (b, i)).ToList();
        
        (GameObject battlefield, int index) selected = battlefieldOptions.Random();
        lastUsedId = selected.index;
        return selected.battlefield;
    }
}
