using System;
using System.Collections.Generic;
using System.Linq;

public class GetUserSelectedEquipment
{
    public Equipment[] Options { get; }
    public Action<Maybe<Equipment>> OnSelected { get; }

    public GetUserSelectedEquipment(IEnumerable<Equipment> options, Action<Maybe<Equipment>> onSelected)
    {
        Options = options.ToArray();
        OnSelected = onSelected;
    }
}
