
using System;

public class BeginTargetSelectionRequested
{
    public Card Card { get; }

    public BeginTargetSelectionRequested(Card c) => Card = c ?? throw new ArgumentNullException((nameof(c)));
}
