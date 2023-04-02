using System;
using System.Collections.Generic;

public class GetUserSelectedEquipmentForDraft : GetUserSelectedEquipment
{
    public GetUserSelectedEquipmentForDraft(IEnumerable<StaticEquipment> options, Action<Maybe<StaticEquipment>> onSelected) 
        : base(options, onSelected) { }
}
