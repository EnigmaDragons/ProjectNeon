using System.Collections.Generic;

public sealed class CardActionSequenceV2 : List<CardActionV2>
{
    public CardActionSequenceV2() {}
    public CardActionSequenceV2(IEnumerable<CardActionV2> actions) : base(actions) {}
}
