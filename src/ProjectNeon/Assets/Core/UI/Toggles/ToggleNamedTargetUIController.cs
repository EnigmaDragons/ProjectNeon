using UnityEngine;

public class ToggleNamedTargetUIController : OnMessage<ToggleNamedTarget>
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private StringReference targetName;
    [SerializeField] private bool alwaysStartInactive;

    private void Awake()
    {
        if (alwaysStartInactive)
            targets.ForEach(t => t.SetActive(false));
    }
    
    protected override void Execute(ToggleNamedTarget msg)
    {
        if (msg.TargetName.Value.Equals(targetName.Value))
            targets.ForEach(t => t.SetActive(!t.activeSelf));
    }
}