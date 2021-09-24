
using UnityEngine;

public class ShowTooltip
{
    public Transform UiSource { get; }
    public string Text { get; }
    public bool ShowBackground { get; }

    public ShowTooltip(Transform uiSource, string text, bool showBackground = true)
    {
        Text = text;
        ShowBackground = showBackground;
    }
}
