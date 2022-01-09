using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class UIStatusBar : StatusBar
{
    [SerializeField] private StatusIcon iconPrototype;
    [SerializeField] private GameObject parentTarget;
    
    private readonly List<StatusIcon> _icons = new List<StatusIcon>();
    
    private void Awake()
    {
        if (parentTarget == null)
            gameObject.DestroyAllChildren();
        Enumerable.Range(0, 8).ForEach(_ => _icons.Add(Instantiate(iconPrototype, parentTarget == null ? transform : parentTarget.transform)));
        UpdateStatuses(new List<CurrentStatusValue>());
    }

    protected override void UpdateStatuses(List<CurrentStatusValue> statuses)
    {
        for (var i = 0; i < Math.Max(statuses.Count, _icons.Count); i++)
        {
            if (i < Math.Min(statuses.Count, _icons.Count))
                _icons[i].Show(statuses[i]);
            else if (i < _icons.Count)
                _icons[i].gameObject.SetActive(false);
        }
    }
}
