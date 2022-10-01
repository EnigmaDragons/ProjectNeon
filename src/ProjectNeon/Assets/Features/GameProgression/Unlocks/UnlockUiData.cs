using UnityEngine;

public class UnlockUiData
{
    public string UnlockType { get; }
    public int ItemId { get; }
    public string Header { get; }
    public string Title { get; }
    public Sprite Image { get; }

    public UnlockUiData(string unlockType, int itemId, string header, string title, Sprite image)
    {
        UnlockType = unlockType;
        ItemId = itemId;
        Header = header;
        Title = title;
        Image = image;
    }

    public override string ToString() => $"{Header} - {Title}";

    public UnlockItemDisplayRecord ToDataRecord() 
        => new UnlockItemDisplayRecord { UnlockType = UnlockType, ItemId = ItemId };
}
