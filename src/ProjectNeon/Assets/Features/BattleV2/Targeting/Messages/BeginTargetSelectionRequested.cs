
using System;

public class BeginTargetSelectionRequested
{
    public Card Card { get; }
    //HACK: drop targets are magically finding the component to call methods on CardPresenter but my non mouse controls have to call this bullshit too and i cant find a better way without rewriting a bunch
    public CardPresenter CardPresenter { get; }

    public BeginTargetSelectionRequested(Card c, CardPresenter cardPresenter)
    {
        Card = c ?? throw new ArgumentNullException((nameof(c)));
        CardPresenter = cardPresenter;
    }
}
