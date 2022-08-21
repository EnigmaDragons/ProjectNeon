using UnityEngine;

public class UnlockUiData
{
    public string Header { get; }
    public string Title { get; }
    public Sprite Image { get; }

    public UnlockUiData(string header, string title, Sprite image)
    {
        Header = header;
        Title = title;
        Image = image;
    }

    public override string ToString() => $"{Header} - {Title}";
}
