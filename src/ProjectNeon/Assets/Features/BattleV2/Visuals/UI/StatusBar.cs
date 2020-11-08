using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StatusBar : OnMessage<MemberStateChanged>
{
    [SerializeField] private StatusIcons icons;

    private List<CurrentStatusValue> _lastStatuses = new List<CurrentStatusValue>();
    private bool _isFirstStatus = true;
    private Member _member;
    
    public StatusBar Initialized(Member m)
    {
        _member = m;
        UpdateUi();
        gameObject.SetActive(true);
        _lastStatuses = new List<CurrentStatusValue>();
        _isFirstStatus = true;
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
        AddStatusIconIfApplicable(statuses, StatType.Resistance, true, v => $"Reduces magic damage taken by {v}");
        AddStatusIconIfApplicable(statuses, TemporalStatType.DoubleDamage, true, v => $"Double Damage for next {v} effects");

        var attackBuffAmount = CeilingInt(_member.State[StatType.Attack] - _member.State.BaseStats.Attack());
        if (attackBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.Attack].Icon, Text = attackBuffAmount.ToString(), Tooltip = $"+{attackBuffAmount} Attack"});
        
        var magicBuffAmount = CeilingInt(_member.State[StatType.Magic] - _member.State.BaseStats.Magic());
        if (magicBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[StatType.Magic].Icon, Text = magicBuffAmount.ToString(), Tooltip = $"{magicBuffAmount} Magic"});
        
        AddStatusIconIfApplicable(statuses, TemporalStatType.Blind, true, v => $"Blinded (guaranteed miss) for {v} Attacks");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Taunt, true, v => $"Taunt for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Stealth, true, v => $"Stealth for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.TurnStun, true, v => $"Stunned for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.CardStun, true, v => $"Stunned for {v} Cards");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Confusion, true, v => $"Confused for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Spellshield, true, v => $"Shields next {v} Magic Attacks");

        var extraCardBuffAmount = CeilingInt(_member.State[StatType.ExtraCardPlays] - _member.State.BaseStats.ExtraCardPlays());
        if (extraCardBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Type = StatType.ExtraCardPlays.ToString(), Icon = icons[StatType.ExtraCardPlays].Icon, Text = extraCardBuffAmount.ToString(), Tooltip = $"Play {extraCardBuffAmount} Extra Cards"});
        
        AddStatusIconIfApplicable(statuses, TemporalStatType.Evade, true, v => $"Evades the next {v} attacks");
        if (_member.State.Damagability() > 1)
            statuses.Add(new CurrentStatusValue { Type = StatType.Damagability.ToString(), Icon = icons[StatType.Damagability].Icon, Tooltip = "Vulnerable (Takes 33% more damage)"});
        
        if (_member.State.Healability() < 1)
            statuses.Add(new CurrentStatusValue { Type = StatusTag.AntiHeal.ToString(), Icon = icons[StatusTag.AntiHeal].Icon, Tooltip = "Anti Heal (Only get 50% healing)"});
        
        if (_member.State.HasStatus(StatusTag.CounterAttack))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.CounterAttack.ToString(), Icon = icons[StatusTag.CounterAttack].Icon, Tooltip = "Counterattack"});
        
        statuses.AddRange(_member.State.StatusesOfType(StatusTag.DamageOverTime)
            .Select(s => new CurrentStatusValue { Type = StatusTag.DamageOverTime.ToString(), Icon = icons[StatusTag.DamageOverTime].Icon, Text = s.RemainingTurns.Select(r => r.ToString(), () => ""), 
                Tooltip = $"Takes {s.Amount.Value} at the Start of the next {s.RemainingTurns.Value} turns"}));
        
        if (_member.State.HasStatus(StatusTag.HealOverTime))
            statuses.Add(new CurrentStatusValue {  Type = StatusTag.HealOverTime.ToString(), Icon = icons[StatusTag.HealOverTime].Icon, Tooltip = "Heals At The Start of Turn" });
        
        if (_member.State.HasStatus(StatusTag.OnHit))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.OnHit.ToString(), Icon = icons[StatusTag.OnHit].Icon, Tooltip = "Special On Hit Effect" });
        
        if (_member.State.HasStatus(StatusTag.StartOfTurnTrigger))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.StartOfTurnTrigger.ToString(),  Icon = icons[StatusTag.StartOfTurnTrigger].Icon, Tooltip = "Start of Turn Effect Trigger" });
        
        if (_member.State.HasStatus(StatusTag.EndOfTurnTrigger))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.EndOfTurnTrigger.ToString(), Icon = icons[StatusTag.EndOfTurnTrigger].Icon, Tooltip = "End of Turn Effect Trigger" });

        if (_member.State.HasStatus(StatusTag.OnDamaged))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.OnDamaged.ToString(), Icon = icons[StatusTag.OnDamaged].Icon, Tooltip = "OnDamaged"});
        
       
        
        UpdateComparisonWithPrevious(statuses);
        UpdateStatuses(statuses);
    }

    private void UpdateComparisonWithPrevious(List<CurrentStatusValue> statuses)
    {
        if (!_isFirstStatus)
            for (var i = 0; i < statuses.Count; i++)
            {
                var newStatus = statuses[i];
                var found = false;
                for (var j = 0; j < _lastStatuses.Count; j++)
                {
                    var oldStatus = _lastStatuses[j];
                    if (oldStatus.IsSameTypeAs(newStatus))
                    {
                        found = true;
                        if (oldStatus.IsChangedFrom(newStatus))
                            newStatus.IsChanged = true;
                    }
                }

                if (!found)
                    newStatus.IsChanged = true;
            }

        _isFirstStatus = false;
        _lastStatuses = statuses;
    }

    private void AddStatusIconIfApplicable(List<CurrentStatusValue> statuses, TemporalStatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.ToString() : "";
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Type = stat.ToString(), Icon = icons[stat].Icon, Text = text, Tooltip =  makeTooltip(value)});
    }

    private void AddStatusIconIfApplicable(List<CurrentStatusValue> statuses, StatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.ToString() : "";
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Type = stat.ToString(), Icon = icons[stat].Icon, Text = text, Tooltip =  makeTooltip(value)});
    }

    protected abstract void UpdateStatuses(List<CurrentStatusValue> statuses);
    private static int CeilingInt(float v) => Convert.ToInt32(Math.Ceiling(v));
}
