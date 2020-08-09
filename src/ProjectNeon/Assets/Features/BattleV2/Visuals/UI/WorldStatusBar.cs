using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class WorldStatusBar : StatusBar
{
    [SerializeField] private StatusIcon iconPrototype;
    [SerializeField] private float xSpacing;
    [SerializeField] private float iconWidth;
    
    private readonly List<StatusIcon> _icons = new List<StatusIcon>();
    
    private void Awake()
    {
        var pos = transform.position;
        Enumerable.Range(0, 8)
            .ForEach(i => _icons.Add(Instantiate(iconPrototype, pos + new Vector3((i * iconWidth) + Math.Max(0, i - 1) * xSpacing, 0, 0), Quaternion.identity, transform)));
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
