using System;
using System.Collections.Generic;
using System.Linq;

public class GetUserSelectedCard
{
    public Card[] Options { get; }
    public Action<Maybe<Card>> OnSelected { get; }

    public GetUserSelectedCard(IEnumerable<Card> options, Action<Maybe<Card>> onSelected)
    {
        Options = options.ToArray();
        OnSelected = onSelected;
    }
}