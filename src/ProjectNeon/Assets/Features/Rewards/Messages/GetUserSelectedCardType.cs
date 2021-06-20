using System;
using System.Collections.Generic;
using System.Linq;

public class GetUserSelectedCardType
{
    public CardTypeData[] Options { get; }
    public Action<Maybe<CardTypeData>> OnSelected { get; }

    public GetUserSelectedCardType(IEnumerable<CardTypeData> options, Action<Maybe<CardTypeData>> onSelected)
    {
        Options = options.ToArray();
        OnSelected = onSelected;
    }
}
