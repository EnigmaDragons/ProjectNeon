using UnityEngine;

public class ShowTooltip
{
    public Vector3 Position { get; }
    public string Text { get; }
    public bool ShowBackground { get; }

    public ShowTooltip(Vector3 position, string text, bool showBackground = true)
    {
        Position = position;
        Text = text;
        ShowBackground = showBackground;
    }
}
