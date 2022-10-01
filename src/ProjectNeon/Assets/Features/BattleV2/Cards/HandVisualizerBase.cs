using System;
using UnityEngine;

public abstract class HandVisualizerBase : MonoBehaviour
{
    public abstract CardPresenter[] ShownCards { get; }
    public abstract void SetOnShownCardsChanged(Action action);
    public abstract void ReProcess();
    public abstract void SetCardPlayingAllowed(bool b);
    public abstract void UpdateVisibleCards();
    public abstract void SelectCard(int indexSelectorIndex);
    public abstract void RecycleCard(int indexSelectorIndex);
}
