using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class StatusBar : OnMessage<MemberStateChanged>
{
    [SerializeField] private StatusIcons icons;
    [SerializeField] private StatusIcon iconPrototype;
    
    private readonly List<StatusIcon> _icons = new List<StatusIcon>();
    private Member _member;
    
    private void Awake()
    {
        Enumerable.Range(0, 8).ForEach(_ => _icons.Add(Instantiate(iconPrototype, transform)));
    }

    public StatusBar Initialized(Member m)
    {
        _member = m;
        UpdateUi();
        gameObject.SetActive(true);
        return this;
    }

    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MemberId == _member.Id)
            UpdateUi();
    }

    private void UpdateUi()
    {
        Debug.Log($"Updating {_member.Name} Status Bar");
        var statuses = new List<CurrentStatusValue>();
        if (_member.State[TemporalStatType.TurnStun] > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[TemporalStatType.TurnStun].Icon, Text = _member.State[TemporalStatType.TurnStun].ToString() });

        for (var i = 0; i < Math.Max(statuses.Count, _icons.Count); i++)
        {
            if (i < statuses.Count)
                _icons[i].Show(statuses[i].Icon, statuses[i].Text);
            else
                _icons[i].gameObject.SetActive(false);
        }
    }
}
