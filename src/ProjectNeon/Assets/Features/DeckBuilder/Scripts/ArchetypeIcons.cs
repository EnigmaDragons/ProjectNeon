using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/ArchetypeIcons")]
public class ArchetypeIcons : ScriptableObject
{
    [SerializeField] private ArchetypeIcon[] icons;

    private Dictionary<string, Sprite> _iconMap;
    
    public Sprite ForArchetypes(HashSet<string> archetypes)
    {
        if (_iconMap == null)
            _iconMap = icons.ToDictionary(x => x.Archetypes.Any() ? string.Join("&", x.Archetypes.Select(a => a.Value).OrderBy(a => a)) : "", x => x.Icon);
        return _iconMap[archetypes.Any() ? string.Join("&", archetypes.OrderBy(a => a)) : ""];
    }
}