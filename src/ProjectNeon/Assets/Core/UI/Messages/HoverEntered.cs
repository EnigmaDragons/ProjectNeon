using UnityEngine;

public class HoverEntered
{
    public Transform UiSource { get; }
    public string ElementName { get; }

    public HoverEntered(Transform uiSource, string elementName)
    {
        UiSource = uiSource;
        ElementName = elementName;
    }
}
