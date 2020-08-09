using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class UIStatusBar : StatusBar
{
    [SerializeField] private StatusIcon iconPrototype;
    
    private readonly List<StatusIcon> _icons = new List<StatusIcon>();
    
    private void Awake()
    {
        Enumerable.Range(0, 8).ForEach(_ => _icons.Add(Instantiate(iconPrototype, transform)));
        UpdateStatuses(new List<CurrentStatusValue>());
    }

    protected override void UpdateStatuses(List<CurrentStatusValue> statuses)
    {
        for (var i = 0; i < Math.Max(statuses.Count, _icons.Count); i++)
        {
            if (i < statuses.Count)
                _icons[i].Show(statuses[i].Icon, statuses[i].Text);
            else
                _icons[i].gameObject.SetActive(false);
        }
    }
}
