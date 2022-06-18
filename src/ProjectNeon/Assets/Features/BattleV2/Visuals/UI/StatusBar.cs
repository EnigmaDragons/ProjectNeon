using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    {
        var s = _member.State.StatusesOfType(statusTag);
        // .DistinctBy((x, i) => x.Status.CustomText.OrDefault(i.ToString))
        // .ToArray();

        var combined = Combine(s, x => x.Status.CustomText.OrDefault(() => defaultText)
            .Replace("[Originator]", battleState.Members.TryGetValue(x.OriginatorId, out var m)
                ? m.UnambiguousName
                : "Originator")
            .Replace("[PrimaryStat]", _member.PrimaryStat().ToString()), x => x.Length > 1 ? x.Length : 0);
        combined.IfPresent(c => statuses.Add(c));
    }

    private Maybe<CurrentStatusValue> Combine(ITemporalState[] statuses, Func<ITemporalState, string> tooltipTemplate, Func<ITemporalState[], int> numberTemplate)
    {
        if (statuses.None())
            return Maybe<CurrentStatusValue>.Missing();

        var first = statuses.First();
        var statusTag = first.Status.Tag;
        var statusValue = new CurrentStatusValue
        {
            Type = statusTag.ToString(),
            Icon = icons[statusTag].Icon,
        };

        var number = numberTemplate(statuses);
        if (number != 0)
            statusValue.Text = number.ToString();

        var originators = statuses.Where(s => s.OriginatorId != _member.Id).Select(s => s.OriginatorId).Distinct().ToArray();
        if (originators.Length == 1)
            statusValue.OriginatorId = originators.First();
        
        var tooltip = new StringBuilder();
        statuses.ForEach(status => tooltip.AppendLine(tooltipTemplate(status)));
        statusValue.Tooltip = tooltip.ToString();
        
        return statusValue;
    }

    private void UpdateUi()
    {
        var statuses = new List<CurrentStatusValue>();

        var cardPlayAmount = CeilingInt(_member.State[StatType.ExtraCardPlays]);
        if (cardPlayAmount > 1)
            statuses.Add(new CurrentStatusValue { Type = StatType.ExtraCardPlays.ToString(), Icon = icons[StatType.ExtraCardPlays].Icon, Text = cardPlayAmount.ToString(), 
                Tooltip = $"Plays {cardPlayAmount} Cards per turn"});
        
        if (_member.State.HasStatus(StatusTag.Invulnerable))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.Invulnerable.ToString(), Icon = icons[StatusTag.Invulnerable].Icon, Tooltip = "Invincible to all Damage" });
        
        AddCustomTextStatusIcons(statuses, StatusTag.CounterAttack, "Counterattack");
        AddCustomTextStatusIcons(statuses, StatusTag.Trap, "Secret Trap Power");
        AddCustomTextStatusIcons(statuses, StatusTag.Augment, "Unknown Augment Power");
                
        foreach (var s in _member.State.CustomStatuses())
            statuses.Add(new CurrentStatusValue { Type = s.Tooltip, Icon = icons[s.IconName].Icon, Text = s.DisplayNumber, Tooltip = s.Tooltip, OriginatorId = s.OriginatorId });;

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
        AddStatusIconIfApplicable(statuses, TemporalStatType.Stun, true, v => $"Stunned for {v} Cards. Reactions disabled.");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Confused, true, v => $"Confused for {v} Turns");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Aegis, true, v => $"Prevents next {v} harmful effects");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Lifesteal, true, v => "Gain HP from your next attack");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Vulnerable, true, v => "Vulnerable (Takes 33% more damage)");
        AddStatusIconIfApplicable(statuses, TemporalStatType.AntiHeal, true, v => "Anti Heal (Only get 50% healing)");
        AddCustomTextStatusIcons(statuses, StatusTag.AfterShielded, "Unknown After Shielded Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnClipUsed, "Unknown On Clip Used Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenBloodied, "Unknown When Bloodied Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenShieldBroken, "Unknown When Shield Broken Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.OnHpDamageDealt, "Unknown On Hp Damage Dealt Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenAllyKilled, "Unknown When Ally Killed Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenAfflicted, "Unknown When Afflicted Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenIgnited, "Unknown When Ignited Effect");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Injury, true, v => $"Received {v} Injuries, applied at end of battle");
        AddStatusIconIfApplicable(statuses, TemporalStatType.Marked, true, v => $"Marked. Subject to Assassination Effects.");
        AddStatusIconIfApplicable(statuses, TemporalStatType.PreventResourceGains, true, v => $"Cannot gain resources for {v} Turns");

        var dotCombined = Combine(_member.State.StatusesOfType(StatusTag.DamageOverTime),
            s => s.Status.CustomText.Select(t => t, $"Takes {s.Amount.Value} at the Start of the next {s.RemainingTurns.Value} turns"),
            s => s.Sum(x => x.Amount.OrDefault(0)));
        dotCombined.IfPresent(d => statuses.Add(d));
        
        if (_member.State.HasStatus(StatusTag.HealOverTime))
            statuses.Add(new CurrentStatusValue {  Type = StatusTag.HealOverTime.ToString(), Icon = icons[StatusTag.HealOverTime].Icon, Tooltip = "Heals At The Start of Turn" });

        if (_member.State[TemporalStatType.Prominent] > 0)
            statuses.Add(new CurrentStatusValue { Type = TemporalStatType.Prominent.ToString(), Icon = icons[TemporalStatType.Prominent].Icon, Tooltip = "Heroes cannot stealth while prominent." });

        AddCustomTextStatusIcons(statuses, StatusTag.WhenHit, "Secret On Hit Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenKilled, "Secret When Killed Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.WhenDamaged, "Secret When Damaged Effect");
        
        AddCustomTextStatusIcons(statuses, StatusTag.StartOfTurnTrigger, "Secret Start of Turn Effect");
        AddCustomTextStatusIcons(statuses, StatusTag.EndOfTurnTrigger, "Secret End of Turn Effect");

        UpdateComparisonWithPrevious(statuses);
        UpdateStatuses(statuses);
    }

    private void AddBuffAmountIconIfApplicable(List<CurrentStatusValue> statuses, StatType statType)
    {
        if (_member.PrimaryStat() == statType)
            return;
        
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
        var text = showNumber && value < 800 // More than 800 is effectively infinite.
            ? value.ToString() 
            : "";
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
