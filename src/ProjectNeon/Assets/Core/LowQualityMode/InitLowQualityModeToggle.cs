using UnityEngine;
using UnityEngine.UI;

public sealed class InitLowQualityModeToggle : OnMessage<LowQualityModeChanged>
{
    [SerializeField] private Toggle toggle;

    private void Awake()
    {
        CurrentLowQualityMode.InitFromPlayerPrefs();
        toggle.SetIsOnWithoutNotify(!CurrentLowQualityMode.IsEnabled);
        toggle.onValueChanged.AddListener(Set);
    }

    private void Set(bool off)
    {
        CurrentLowQualityMode.Set(!off);
        toggle.SetIsOnWithoutNotify(off);
    }

    protected override void Execute(LowQualityModeChanged msg)
    {
        toggle.SetIsOnWithoutNotify(msg.IsEnabled);
    }
}
