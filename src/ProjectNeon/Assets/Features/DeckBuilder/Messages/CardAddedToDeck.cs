using UnityEngine;

public class CardAddedToDeck
{
    public Transform UiSource { get; }

    public CardAddedToDeck(Transform uiSource) => UiSource = uiSource;
}
