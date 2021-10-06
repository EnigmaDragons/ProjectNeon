using UnityEngine;

public class HoverExited
{
    public Transform UiSource { get; }
    public string ElementName { get; }

    public HoverExited(Transform uiSource, string elementName)
    {
        UiSource = uiSource;
        ElementName = elementName;
    }
}
