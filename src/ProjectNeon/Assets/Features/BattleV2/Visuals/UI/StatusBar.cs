using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StatusBar : OnMessage<MemberStateChanged>
{
    [SerializeField] private BattleState battleState;
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

    private void AddCustomTextStatusIcons(List<CurrentStatusValue> statuses, StatusTag statusTag, string defaultText = "Unknown") 
        => _member.State.StatusesOfType(statusTag)
            .DistinctBy((x, i) => x.Status.CustomText.OrDefault(i.ToString))
            .ForEach(s => statuses.Add(new CurrentStatusValue { 
                Type = statusTag.ToString(), 
                Icon = icons[statusTag].Icon, 
                Tooltip = s.Status.CustomText.OrDefault(() => defaultText)
                    .Replace("[Originator]", battleState.Members.TryGetValue(s.OriginatorId, out var m) 
                        ? m.ToString() 
                        : "Originator"),
                OriginatorId = s.OriginatorId
            }));

    private void UpdateUi()
    {
        var statuses = new List<CurrentStatusValue>();

        if (_member.State.HasStatus(StatusTag.Invulnerable))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.Invulnerable.ToString(), Icon = icons[StatusTag.Invulnerable].Icon, Tooltip = "Invincible to all Damage" });
        
        if (_member.State.HasStatus(StatusTag.CounterAttack))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.CounterAttack.ToString(), Icon = icons[StatusTag.CounterAttack].Icon, Tooltip = "Counterattack"});
        
        AddCustomTextStatusIcons(statuses, StatusTag.Trap, "Secret Trap Power");
        AddCustomTextStatusIcons(statuses, StatusTag.Augment, "Unknown Augment Power");
        
        AddStatusIconIfApplicable(statuses, TemporalStatType.Dodge, true, v => $"Dodges the next {v} attacks");
        AddStatusIconIfApplicable(statuses, StatType.Armor, true, v => $"Reduces attack damage taken by {v}");
        AddStatusIconIfApplicable(statuses, StatType.Resistance, true, v => $"Reduces magic damage taken by {v}");
        AddStatusIconIfApplicable(statuses, TemporalStatType.DoubleDamage, true, v => $"Double Damage for next {v} effects");
        AddBuffAmountIconIfApplicable(statuses, StatType.Attack);
        AddBuffAmountIconIfApplicable(statuses, StatType.Magic);        
        AddBuffAmountIconIfApplicable(statuses, StatType.Leadership);
        AddBuffAmountIconIfApplicable(statuses, StatType.Economy);
        AddStatusIconIfApplicable(statuses, TemporalStatType.Blind, true, v => $"Blinded (guaranteed miss) for {v} Attacks");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Inhibit, true, v => $"Inhibited (guaranteed miss) for {v} Spells");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Taunt, true, v => $"Taunt for {v} Turns");
        if (_member.State[TemporalStatType.Stealth] > 0)
            statuses.Add(new CurrentStatusValue { Type = TemporalStatType.Stealth.ToString(), Icon = icons[TemporalStatType.Stealth].Icon, Tooltip = "Stealth"});
        AddStatusIconIfApplicable(statuses, TemporalStatType.Disabled, true, v => $"Disabled for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.CardStun, true, v => $"Stunned for {v} Cards. Reactions disabled.");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Confused, true, v => $"Confused for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Aegis, true, v => $"Prevents next {v} harmful effects");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Lifesteal, true, v => "Gain HP from your next attack");
        AddCustomTextStatusIcons(statuses, StatusTag.OnClipUsed, "Unknown On Clip Used Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnBloodied, "Unknown On Bloodied Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnShieldBroken, "Unknown On Shield Broken Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnHpDamageDealt, "Unknown On Hp Damage Dealt Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnAllyDeath, "Unknown On Ally Death Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnAfflicted, "Unknown On Afflicted Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnIgnited, "Unknown On Ignited Effect");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Injury, true, v => $"Received {v} Injuries, applied at end of battle");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Marked, true, v => $"Marked. Subject to Assassination Effects.");
        AddStatusIconIfApplicable(statuses, TemporalStatType.PreventResourceGains, true, v => $"Cannot gain resources for {v} Turns");

        var extraCardBuffAmount = CeilingInt(_member.State[StatType.ExtraCardPlays] - _member.State.BaseStats.ExtraCardPlays());
        if (extraCardBuffAmount != 0)
            statuses.Add(new CurrentStatusValue { Type = StatType.ExtraCardPlays.ToString(), Icon = icons[StatType.ExtraCardPlays].Icon, Text = extraCardBuffAmount.ToString(), Tooltip = $"Play {extraCardBuffAmount} Extra Cards"});
        
        if (_member.State.Damagability() > 1)
            statuses.Add(new CurrentStatusValue { Type = StatType.Damagability.ToString(), Icon = icons[StatType.Damagability].Icon, Tooltip = "Vulnerable (Takes 33% more damage)"});
        
        if (_member.State.Healability() < 1)
            statuses.Add(new CurrentStatusValue { Type = StatusTag.AntiHeal.ToString(), Icon = icons[StatusTag.AntiHeal].Icon, Tooltip = "Anti Heal (Only get 50% healing)"});
        
        statuses.AddRange(_member.State.StatusesOfType(StatusTag.DamageOverTime)
            .Select(s => new CurrentStatusValue { Type = StatusTag.DamageOverTime.ToString(), Icon = icons[StatusTag.DamageOverTime].Icon, Text = s.RemainingTurns.Select(r => r.ToString(), () => ""), 
                Tooltip = $"Takes {s.Amount.Value} at the Start of the next {s.RemainingTurns.Value} turns", OriginatorId = s.OriginatorId }));
        
        if (_member.State.HasStatus(StatusTag.HealOverTime))
            statuses.Add(new CurrentStatusValue {  Type = StatusTag.HealOverTime.ToString(), Icon = icons[StatusTag.HealOverTime].Icon, Tooltip = "Heals At The Start of Turn" });

        if (_member.State[TemporalStatType.Prominent] > 0)
            statuses.Add(new CurrentStatusValue { Type = TemporalStatType.Prominent.ToString(), Icon = icons[TemporalStatType.Prominent].Icon, Tooltip = "Heroes cannot stealth while prominent." });

        AddCustomTextStatusIcons(statuses, StatusTag.OnHit, "Secret On Hit Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnDeath, "Secret On Death Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnDamaged, "Secret On Damaged Effect");
        
        foreach (var s in _member.State.CustomStatuses())
            statuses.Add(new CurrentStatusValue { Type = s.Tooltip, Icon = icons[s.IconName].Icon, Text = s.DisplayNumber, Tooltip = s.Tooltip, OriginatorId = s.OriginatorId });;
        
        if (_member.State.HasStatus(StatusTag.StartOfTurnTrigger))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.StartOfTurnTrigger.ToString(),  Icon = icons[StatusTag.StartOfTurnTrigger].Icon, Tooltip = "Start of Turn Effect Trigger" });
        
        if (_member.State.HasStatus(StatusTag.EndOfTurnTrigger))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.EndOfTurnTrigger.ToString(), Icon = icons[StatusTag.EndOfTurnTrigger].Icon, Tooltip = "End of Turn Effect Trigger" });
        
        UpdateComparisonWithPrevious(statuses);
        UpdateStatuses(statuses);
    }

    private void AddBuffAmountIconIfApplicable(List<CurrentStatusValue> statuses, StatType statType)
    {
        var buffAmount = CeilingInt(_member.State[statType] - _member.State.BaseStats[statType]);
        if (buffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[statType].Icon, Text = buffAmount.ToString(), Tooltip = $"{Sign(buffAmount)}{buffAmount} {statType}"});
    }

    private string Sign(float amount) => amount > 0 ? "+" : "";

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
