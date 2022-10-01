using UnityEngine;

public class ToggleNamedTargetUIController : OnMessage<ToggleNamedTarget, HideNamedTarget, ShowNamedTarget>
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

    protected override void Execute(HideNamedTarget msg)
    {
        if (msg.TargetName.Value.Equals(targetName.Value))
            targets.ForEach(t => t.SetActive(false));
    }

    protected override void Execute(ShowNamedTarget msg)
    {
        if (msg.TargetName.Value.Equals(targetName.Value))
            targets.ForEach(t => t.SetActive(true));
    }
}