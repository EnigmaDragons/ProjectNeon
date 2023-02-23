using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class StatusBar : OnMessage<MemberStateChanged>, ILocalizeTerms
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

    private void AddCustomTextStatusIcons(Dictionary<StatusTag, ITemporalState[]> states, List<CurrentStatusValue> statuses, StatusTag statusTag, string defaultText)
    {
        var s = states.ValueOrDefault(statusTag, Array.Empty<ITemporalState>());

        var combined = Combine(s, x => x.Status.CustomText.OrDefault(() => defaultText)
            .Replace("{[Originator]}", battleState.Members.TryGetValue(x.OriginatorId, out var m)
                ? m.TeamType == TeamType.Enemies ? $"{m.NameTerm.ToLocalized()} {m.Id}" : m.NameTerm.ToLocalized()
                : "Tooltips/Originator".ToLocalized())
            .Replace("{[PrimaryStat]}", _member.PrimaryStat().GetString()), x => x.Length > 1 ? x.Length : 0);
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
            Type = statusTag.GetString(),
            Icon = icons[statusTag].Icon,
        };

        var number = numberTemplate(statuses);
        if (number != 0)
            statusValue.Text = number.GetString();

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
        var memberStatesByTag = _member.State.TemporalStates.GroupBy(s => s.Status.Tag).SafeToDictionary(t => t.Key, t => t.ToArray());

        var cardPlayAmount = CeilingInt(_member.State[StatType.ExtraCardPlays]);
        if (cardPlayAmount > 1)
            statuses.Add(new CurrentStatusValue { Type = StatType.ExtraCardPlays.GetString(), Icon = icons[StatType.ExtraCardPlays].Icon, Text = cardPlayAmount.GetString(), 
                Tooltip = string.Format("Tooltips/PlaysCardsPerTurn".ToLocalized(), cardPlayAmount) });
        
        if (_member.State.HasStatus(StatusTag.Invulnerable))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.Invulnerable.GetString(), Icon = icons[StatusTag.Invulnerable].Icon, Tooltip = "Tooltips/Invincible".ToLocalized() });
        
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.CounterAttack, "Tooltips/Counterattack".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.Trap, "Tooltips/SecretTrapPower".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.Augment, "Tooltips/UnknownAugmentPower".ToLocalized());
                
        foreach (var s in _member.State.CustomStatuses())
            statuses.Add(new CurrentStatusValue { Type = s.Tooltip, Icon = icons[s.IconName].Icon, Text = s.DisplayNumber, Tooltip = s.Tooltip, OriginatorId = s.OriginatorId });;

        AddStatusIconIfApplicable(statuses, TemporalStatType.Dodge, true, v => string.Format("Tooltips/DodgeAttacks".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, StatType.Armor, true, v => string.Format("Tooltips/ReduceAttackDamage".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, StatType.Resistance, true, v => string.Format("Tooltips/ReduceMagicDamage".ToLocalized(), v.ToString()));
        AddNegativeStatusIconIfApplicable(statuses, StatType.Armor, true, v => string.Format("Tooltips/IncreaseAttackDamage".ToLocalized(), v.ToString()));
        AddNegativeStatusIconIfApplicable(statuses, StatType.Resistance, true, v => string.Format("Tooltips/IncreaseMagicDamage".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.DoubleDamage, true, v => string.Format("Tooltips/DoubleDamage".ToLocalized(), v.ToString()));
        AddBuffAmountIconIfApplicable(statuses, StatType.Attack);
        AddBuffAmountIconIfApplicable(statuses, StatType.Magic);        
        AddBuffAmountIconIfApplicable(statuses, StatType.Leadership);
        AddBuffAmountIconIfApplicable(statuses, StatType.Economy);
        AddStatusIconIfApplicable(statuses, TemporalStatType.Blind, true, v => string.Format("Tooltips/Blinded".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Inhibit, true, v => string.Format("Tooltips/Inhibited".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Taunt, true, v => string.Format("Tooltips/Taunt".ToLocalized(), v.ToString()));
        if (_member.State[TemporalStatType.Stealth] > 0)
            statuses.Add(new CurrentStatusValue { Type = TemporalStatType.Stealth.GetString(), Icon = icons[TemporalStatType.Stealth].Icon, Tooltip = "Tooltips/Stealth".ToLocalized()});
        AddStatusIconIfApplicable(statuses, TemporalStatType.Disabled, true, v => string.Format("Tooltips/Disabled".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Stun, true, v => string.Format("Tooltips/Stunned".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Confused, true, v => string.Format("Tooltips/Confused".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Aegis, true, v => string.Format("Tooltips/Aegis".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Lifesteal, true, v => "Tooltips/Lifesteal".ToLocalized());
        AddStatusIconIfApplicable(statuses, TemporalStatType.Vulnerable, true, v => "Tooltips/Vulnerable".ToLocalized());
        AddStatusIconIfApplicable(statuses, TemporalStatType.AntiHeal, true, v => "Tooltips/AntiHeal".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.AfterShielded, "Tooltips/AfterShielded".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.OnClipUsed, "Tooltips/ClipUsedEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenBloodied, "Tooltips/BloodiedEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenShieldBroken, "Tooltips/ShieldBrokenEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.OnHpDamageDealt, "Tooltips/HpDamageDealtEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenAllyKilled, "Tooltips/AllyKilledEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenAfflicted, "Tooltips/AfflictedEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenIgnited, "Tooltips/IgnitedEffect".ToLocalized());
        AddStatusIconIfApplicable(statuses, TemporalStatType.Injury, true, v => string.Format("Tooltips/ReceivedInjuries".ToLocalized(), v.ToString()));
        AddStatusIconIfApplicable(statuses, TemporalStatType.Marked, true, v => "Tooltips/Marked".ToLocalized());
        AddStatusIconIfApplicable(statuses, TemporalStatType.PreventResourceGains, true, v => string.Format("Tooltips/PreventResourceGains".ToLocalized(), v.ToString()));

        var dotCombined = Combine(memberStatesByTag.ValueOrDefault(StatusTag.DamageOverTime, Array.Empty<ITemporalState>()),
            s => s.Status.CustomText.Select(t => t, string.Format("Tooltips/DamageOverTime".ToLocalized(), s.Amount.Value, s.RemainingTurns.Value)),
            s => s.Sum(x => x.Amount.OrDefault(0)));
        dotCombined.IfPresent(d => statuses.Add(d));
        
        if (_member.State.HasStatus(StatusTag.HealOverTime))
            statuses.Add(new CurrentStatusValue { Type = StatusTag.HealOverTime.GetString(), Icon = icons[StatusTag.HealOverTime].Icon, Tooltip = "Tooltips/HealsTurnStart".ToLocalized() });

        if (_member.State[TemporalStatType.Prominent] > 0)
            statuses.Add(new CurrentStatusValue { Type = TemporalStatType.Prominent.GetString(), Icon = icons[TemporalStatType.Prominent].Icon, Tooltip = "Tooltips/CannotStealthProminent".ToLocalized() });

        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenHit, "Tooltips/OnHitEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenKilled, "Tooltips/WhenKilledEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenDamaged, "Tooltips/WhenDamagedEffect".ToLocalized());
        
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.StartOfTurnTrigger, "Tooltips/TurnStartEffect".ToLocalized());
        AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.EndOfTurnTrigger, "Tooltips/TurnEndEffect".ToLocalized());

        UpdateComparisonWithPrevious(statuses);
        UpdateStatuses(statuses);
    }

    private void AddBuffAmountIconIfApplicable(List<CurrentStatusValue> statuses, StatType statType)
    {
        if (_member.PrimaryStat() == statType)
            return;
        
        var buffAmount = CeilingInt(_member.State[statType] - _member.State.BaseStats[statType]);
        if (buffAmount != 0)
            statuses.Add(new CurrentStatusValue { Icon = icons[statType].Icon, Text = buffAmount.GetString(), Tooltip = $"{Sign(buffAmount)}{buffAmount} {statType.GetTerm().ToLocalized()}"});
    }

    private string Sign(float amount) => amount > 0 ? "+" : string.Empty;

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
            ? value.GetCeilingIntString() 
            : string.Empty;
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Type = stat.GetString(), Icon = icons[stat].Icon, Text = text, Tooltip = makeTooltip(value)});
    }
    
    private void AddStatusIconIfApplicable(List<CurrentStatusValue> statuses, StatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.GetCeilingIntString() : string.Empty;
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Type = stat.GetString(), Icon = icons[stat].Icon, Text = text, Tooltip =  makeTooltip(value)});
    }
    
    private void AddNegativeStatusIconIfApplicable(List<CurrentStatusValue> statuses, StatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.GetCeilingIntString() : string.Empty;
        if (value < 0)
            statuses.Add(new CurrentStatusValue { Type = stat.GetString(), Icon = icons[stat].Icon, Text = text, Tooltip = makeTooltip(-value)});
    }

    protected abstract void UpdateStatuses(List<CurrentStatusValue> statuses);
    private static int CeilingInt(float v) => Convert.ToInt32(Math.Ceiling(v));

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Tooltips/Originator", 
            "Tooltips/PlaysCardsPerTurn",
            "Tooltips/Invincible",
            "Tooltips/Counterattack",
            "Tooltips/SecretTrapPower",
            "Tooltips/UnknownAugmentPower",
            "Tooltips/DodgeAttacks",
            "Tooltips/ReduceAttackDamage",
            "Tooltips/ReduceMagicDamage",
            "Tooltips/DoubleDamage",
            "Tooltips/Blinded",
            "Tooltips/Inhibited",
            "Tooltips/Taunt",
            "Tooltips/Stealth",
            "Tooltips/Disabled",
            "Tooltips/Stunned",
            "Tooltips/Confused",
            "Tooltips/Aegis",
            "Tooltips/Lifesteal",
            "Tooltips/Vulnerable",
            "Tooltips/AntiHeal",
            "Tooltips/AfterShielded",
            "Tooltips/ClipUsedEffect",
            "Tooltips/BloodiedEffect",
            "Tooltips/ShieldBrokenEffect",
            "Tooltips/HpDamageDealtEffect",
            "Tooltips/AllyKilledEffect",
            "Tooltips/AfflictedEffect",
            "Tooltips/IgnitedEffect",
            "Tooltips/ReceivedInjuries",
            "Tooltips/Marked",
            "Tooltips/PreventResourceGains",
            "Tooltips/DamageOverTime",
            "Tooltips/HealsTurnStart",
            "Tooltips/CannotStealthProminent",
            "Tooltips/OnHitEffect",
            "Tooltips/WhenKilledEffect",
            "Tooltips/WhenDamagedEffect",
            "Tooltips/TurnStartEffect",
            "Tooltips/TurnEndEffect"
        };
}
