using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusBar : OnMessage<MemberStateChanged>
{
    [SerializeField] private StatusIcons icons;
    
    private Member _member;
    
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
        var statuses = new List<CurrentStatusValue>();
        
        if (_member.State[TemporalStatType.Taunt] > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[TemporalStatType.Taunt].Icon, Text = _member.State[TemporalStatType.Taunt].ToString(), Tooltip = "Taunt" });

        var armor = _member.State[StatType.Armor]; 
        if (armor > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.Armor].Icon, Text = armor.ToString(), Tooltip = $"Reduces Attack Damage by {armor}"});
        
        if (_member.State[TemporalStatType.TurnStun] > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[TemporalStatType.TurnStun].Icon, Text = _member.State[TemporalStatType.TurnStun].ToString() });

        if (_member.State[TemporalStatType.CardStun] > 0)
            statuses.Add(new CurrentStatusValue {Icon = icons[TemporalStatType.TurnStun].Icon, Text = _member.State[TemporalStatType.CardStun].ToString() });

        var attackBuffAmount = CeilingInt(_member.State[StatType.Attack]) - _member.State.BaseStats.Attack();
        if (attackBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.Attack].Icon, Text = attackBuffAmount.ToString() });

        var extraCardBuffAmount = CeilingInt(_member.State[StatType.ExtraCardPlays] - _member.State.BaseStats.ExtraCardPlays());
        if (extraCardBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.ExtraCardPlays].Icon, Text = extraCardBuffAmount.ToString() });
        
        if (_member.State[StatType.Damagability] > 1f)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.Damagability].Icon, Text = "" });

        if (_member.State.HasStatus(StatusTag.CounterAttack))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.CounterAttack].Icon, Text = "" });
        
        if (_member.State.HasStatus(StatusTag.DamageOverTime))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.DamageOverTime].Icon, Text = "" });
        
        if (_member.State.HasStatus(StatusTag.HealOverTime))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.HealOverTime].Icon, Text = "" });
        
        if (_member.State[TemporalStatType.Evade] > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[TemporalStatType.Evade].Icon, Text = _member.State[TemporalStatType.Evade].ToString() });
        
        if (_member.State.HasStatus(StatusTag.OnHit))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.OnHit].Icon, Text = "" });
        
        UpdateStatuses(statuses);
    }

    protected abstract void UpdateStatuses(List<CurrentStatusValue> statuses);
    private static int CeilingInt(float v) => Convert.ToInt32(Math.Ceiling(v));
}
