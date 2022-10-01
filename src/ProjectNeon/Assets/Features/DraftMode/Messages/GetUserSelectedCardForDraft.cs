
using System;
using System.Collections.Generic;

public class GetUserSelectedCardForDraft : GetUserSelectedCard
{
    public GetUserSelectedCardForDraft(IEnumerable<Card> options, Action<Maybe<Card>> onSelected) 
        : base(options, onSelected) { }
}