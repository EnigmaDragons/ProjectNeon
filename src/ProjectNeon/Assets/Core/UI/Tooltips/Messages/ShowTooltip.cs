
public class ShowTooltip
{
    public string Text { get; }
    public bool ShowBackground { get; }

    public ShowTooltip(string text, bool showBackground = false)
    {
        Text = text;
        ShowBackground = showBackground;
    }
}
