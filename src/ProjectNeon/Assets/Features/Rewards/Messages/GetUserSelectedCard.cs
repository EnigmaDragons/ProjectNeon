using System;
using System.Collections.Generic;
using System.Linq;

public class GetUserSelectedCard
{
    public CardTypeData[] Options { get; }
    public Action<Maybe<CardTypeData>> OnSelected { get; }

    public GetUserSelectedCard(IEnumerable<CardTypeData> options, Action<Maybe<CardTypeData>> onSelected)
    {
        Options = options.ToArray();
        OnSelected = onSelected;
    }
}
