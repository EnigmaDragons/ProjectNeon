using UnityEngine;

public class UnlockUiData
{
    public string UnlockType { get; }
    public int ItemId { get; }
    public string HeaderTerm { get; }
    public string TitleTerm { get; }
    public Sprite Image { get; }

    public UnlockUiData(string unlockType, int itemId, string headerTerm, string titleTerm, Sprite image)
    {
        UnlockType = unlockType;
        ItemId = itemId;
        HeaderTerm = headerTerm;
        TitleTerm = titleTerm;
        Image = image;
    }

    public override string ToString() => $"{HeaderTerm} - {TitleTerm}";

    public UnlockItemDisplayRecord ToDataRecord() 
        => new UnlockItemDisplayRecord { UnlockType = UnlockType, ItemId = ItemId };
}
