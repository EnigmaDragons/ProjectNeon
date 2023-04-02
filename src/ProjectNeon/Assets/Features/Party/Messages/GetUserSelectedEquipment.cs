using System;
using System.Collections.Generic;
using System.Linq;

public class GetUserSelectedEquipment
{
    public StaticEquipment[] Options { get; }
    public Action<Maybe<StaticEquipment>> OnSelected { get; }

    public GetUserSelectedEquipment(IEnumerable<StaticEquipment> options, Action<Maybe<StaticEquipment>> onSelected)
    {
        Options = options.ToArray();
        OnSelected = onSelected;
    }
}
