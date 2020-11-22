using System;
using System.Collections.Generic;
using System.Linq;

public class GetUserSelectedCard
{
    public CardType[] Options { get; }
    public Action<Maybe<CardType>> OnSelected { get; }

    public GetUserSelectedCard(IEnumerable<CardType> options, Action<Maybe<CardType>> onSelected)
    {
        Options = options.ToArray();
        OnSelected = onSelected;
    }
}
