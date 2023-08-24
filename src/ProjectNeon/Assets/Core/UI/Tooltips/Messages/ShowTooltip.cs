using UnityEngine;

public class ShowTooltip
{
    public Transform Position { get; }
    public string Text { get; }
    public bool ShowBackground { get; }

    public ShowTooltip(Transform position, string text, bool showBackground = true)
    {
        Position = position;
        Text = text;
        ShowBackground = showBackground;
    }
}
