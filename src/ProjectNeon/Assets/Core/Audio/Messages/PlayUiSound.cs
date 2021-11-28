using UnityEngine;

public class PlayUiSound
{
    public Transform UiSource { get; }
    public string Name { get; }

    public PlayUiSound(string name, Transform uiSource)
    {
        UiSource = uiSource;
        Name = name;
    }
}
