using UnityEngine;
using UnityEngine.UI;

public sealed class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private DisplaySettings display;

    private void Awake()
    {
        Set(Screen.fullScreen);
        toggle.onValueChanged.AddListener(Set);
    }

    private void Set(bool on)
    {
        toggle.isOn = on;
        display.SetFullscreen(on);
    }
}
