using UnityEngine;

public class CharacterSpawnMarker : MonoBehaviour
{
    [SerializeField] private StringReference characterAlias;

    public string Alias => characterAlias;

    public GameObject SpawnTo(GameObject o)
    {
        Clear();
        return Instantiate(o, transform);
    }

    public void Clear() => transform.DestroyAllChildrenImmediate();
}
