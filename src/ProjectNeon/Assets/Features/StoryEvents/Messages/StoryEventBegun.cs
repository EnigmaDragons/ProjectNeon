using UnityEngine;

public class StoryEventBegun
{
    public Transform UiSource { get; }

    public StoryEventBegun(Transform uiSource) => UiSource = uiSource;
}
