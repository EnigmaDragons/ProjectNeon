﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/ArchetypeTints")]
public class ArchetypeTints : ScriptableObject
{
    [SerializeField] private ArchetypeTint[] tints;

    private DictionaryWithDefault<string, Color> _tintMap;
    
    public Color ForArchetypes(HashSet<string> archetypes)
    {
        if (_tintMap == null)
            _tintMap = new DictionaryWithDefault<string, Color>(Color.white, tints.ToDictionary(x => x.Archetypes.Any() 
                ? string.Join("&", x.Archetypes.Select(a => a.Value).OrderBy(a => a)) 
                : "", x => x.Tint));
        return _tintMap[archetypes.Any() ? string.Join("&", archetypes.OrderBy(a => a)) : ""];
    }
}