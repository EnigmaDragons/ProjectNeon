using System;
using System.Collections.Generic;

public class GetUserSelectedEquipmentForDraft : GetUserSelectedEquipment
{
    public GetUserSelectedEquipmentForDraft(IEnumerable<Equipment> options, Action<Maybe<Equipment>> onSelected) 
        : base(options, onSelected) { }
}
