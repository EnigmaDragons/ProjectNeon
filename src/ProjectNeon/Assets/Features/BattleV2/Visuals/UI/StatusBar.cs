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
        
        AddStatusIconIfApplicable(statuses, StatType.Armor, true, v => $"Reduces attack damage taken by {v}");
        AddStatusIconIfApplicable(statuses, StatType.Attack, true, v => $"+{v} Attack");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Taunt, true, v => $"Taunt for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.TurnStun, true, v => $"Stunned for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.CardStun, true, v => $"Stunned for {v} Cards");

        var extraCardBuffAmount = CeilingInt(_member.State[StatType.ExtraCardPlays] - _member.State.BaseStats.ExtraCardPlays());
        if (extraCardBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.ExtraCardPlays].Icon, Text = extraCardBuffAmount.ToString(), Tooltip = $"Play {extraCardBuffAmount} Extra Cards"});
        
        AddStatusIconIfApplicable(statuses, StatType.Damagability, false, v => $"Vulnerable (Takes 33% more damage)");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Evade, true, v => $"Evades the next {v} attacks");
        
        if (_member.State.HasStatus(StatusTag.CounterAttack))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.CounterAttack].Icon, Tooltip = "Counterattack"});
        
        if (_member.State.HasStatus(StatusTag.DamageOverTime))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.DamageOverTime].Icon, Tooltip = "Takes Damage At The Start of Turn"});
        
        if (_member.State.HasStatus(StatusTag.HealOverTime))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.HealOverTime].Icon, Tooltip = "Heals At The Start of Turn" });
        
        if (_member.State.HasStatus(StatusTag.OnHit))
            statuses.Add(new CurrentStatusValue { Icon = icons[StatusTag.OnHit].Icon, Tooltip = "Has On Hit Effect" });
        
        UpdateStatuses(statuses);
    }

    private void AddStatusIconIfApplicable(List<CurrentStatusValue> statuses, TemporalStatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.ToString() : "";
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[stat].Icon, Text = text, Tooltip =  makeTooltip(value)});
    }

    private void AddStatusIconIfApplicable(List<CurrentStatusValue> statuses, StatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.ToString() : "";
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[stat].Icon, Text = text, Tooltip =  makeTooltip(value)});
    }

    protected abstract void UpdateStatuses(List<CurrentStatusValue> statuses);
    private static int CeilingInt(float v) => Convert.ToInt32(Math.Ceiling(v));
}
