using UnityEngine;

public sealed class SingletonObject : CrossSceneSingleInstance
{
    [SerializeField] private string uniqueTag;
    
    protected override string UniqueTag => uniqueTag;
    protected override void OnAwake() {}
}
