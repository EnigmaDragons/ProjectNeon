using UnityEngine;

public abstract class CutsceneSetting : ScriptableObject
{
    public abstract string GetDisplayName();
    public abstract void SpawnTo(GameObject parent);
}
