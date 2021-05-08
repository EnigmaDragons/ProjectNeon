using System;

[Flags]
public enum CardLocation
{
    Nowhere = 0,
    Hand = 1,
    Deck = 2,
    Discard = 4
}