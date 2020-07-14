using System;
using UnityEngine;

[CreateAssetMenu]
public sealed class DisplaySettings : ScriptableObject
{
    [SerializeField] private bool isFullscreen = true;
    [SerializeField] private Vector2Int screenSize;
    
    public bool IsFullscreen => isFullscreen;
    private string Mode => (isFullscreen ? "Fullscreen" : "Windowed");

    public void InitWithoutChanging() => screenSize = new Vector2Int(Screen.width, Screen.height);

    public void SetFullscreen(bool on) => UpdateAfter(() => isFullscreen = on);
    public void ToggleFullscreen() => UpdateAfter(() => isFullscreen = !isFullscreen);
    public Vector2Int CurrentScreenSize => screenSize;
    public void SetResolution(Resolution r) => UpdateAfter(() => screenSize = new Vector2Int(r.width, r.height));
    
    private void UpdateAfter(Action set)
    {
        var old = $"{Mode}-{screenSize.x}x{screenSize.y}";
        set();
        var newHash = $"{Mode}-{screenSize.x}x{screenSize.y}";
        if (newHash != old)
        {
            Screen.SetResolution(screenSize.x, screenSize.y, isFullscreen);
            Debug.Log($"Changed Display/ Old: {old}. New: {newHash}");
        }
    }
}
