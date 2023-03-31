using System;
using System.Collections.Generic;
using System.Linq;

public static class ArrayBoxExtensions
{
    public static CardTypeDataBox[] ToBoxes(this IEnumerable<CardTypeData> cardTypes)
        => cardTypes?.Where(x => x != null)?.Select(x => new CardTypeDataBox(x))?.ToArray() ?? Array.Empty<CardTypeDataBox>();

    public static CardTypeData[] FromBoxes(this IEnumerable<CardTypeDataBox> boxes)
        => boxes?.Where(x => x != null && x.IsBoxFilled())?.Select(x => x.Get())?.ToArray() ?? Array.Empty<CardTypeData>();
    
    public static EquipmentBox[] ToBoxes(this IEnumerable<Equipment> equips)
        => equips?.Where(x => x != null)?.Select(x => new EquipmentBox(x))?.ToArray() ?? Array.Empty<EquipmentBox>();

    public static Equipment[] FromBoxes(this IEnumerable<EquipmentBox> boxes)
        => boxes?.Where(x => x != null && x.IsBoxFilled())?.Select(x => x.Get())?.ToArray() ?? Array.Empty<Equipment>();
    
    public static HeroCharacterBox[] ToBoxes(this IEnumerable<HeroCharacter> equips)
        => equips?.Where(x => x != null)?.Select(x => new HeroCharacterBox(x))?.ToArray() ?? Array.Empty<HeroCharacterBox>();

    public static HeroCharacter[] FromBoxes(this IEnumerable<HeroCharacterBox> boxes)
        => boxes?.Where(x => x != null && x.IsBoxFilled())?.Select(x => x.Get())?.ToArray() ?? Array.Empty<HeroCharacter>();
}