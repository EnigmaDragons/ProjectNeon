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
        foreach (var status in statuses)
            tooltip.AppendLine(tooltipTemplate(status));
        statusValue.Tooltip = tooltip.ToString();
        
        return statusValue;
    }

    private void UpdateUi()
    {
        try
        {
            var statuses = new List<CurrentStatusValue>();
            var memberStatesByTag = _member.State.TemporalStates.GroupBy(s => s.Status.Tag)
                .SafeToDictionary(t => t.Key, t => t.ToArray());
            
            if (_member.IsHasty)
                statuses.Add(new CurrentStatusValue
                {
                    Type = "Hasty",
                    Icon = icons["Hasty"].Icon,
                    Text = "",
                    Tooltip = "Tooltips/Hasty".ToLocalized() 
                });
            
            var cardPlayAmount = CeilingInt(_member.State[StatType.ExtraCardPlays]);
            if (cardPlayAmount > 1)
                statuses.Add(new CurrentStatusValue
                {
                    Type = StatType.ExtraCardPlays.GetString(), 
                    Icon = icons[StatType.ExtraCardPlays].Icon,
                    Text = cardPlayAmount.GetString(),
                    Tooltip = "Tooltips/PlaysCardsPerTurn".ToLocalized().SafeFormatWithDefault("Plays {0} Cards per turn", cardPlayAmount)
                });

            if (_member.State.HasStatus(StatusTag.Invulnerable))
                statuses.Add(new CurrentStatusValue
                {
                    Type = StatusTag.Invulnerable.GetString(), 
                    Icon = icons[StatusTag.Invulnerable].Icon,
                    Tooltip = "Tooltips/Invincible".ToLocalized()
                });

            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.CounterAttack,
                () => "Tooltips/Counterattack".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.Trap,
                () => "Tooltips/SecretTrapPower".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.Augment,
                () => "Tooltips/UnknownAugmentPower".ToLocalized());

            foreach (var s in _member.State.CustomStatuses())
                statuses.Add(new CurrentStatusValue
                {
                    Type = s.Tooltip, Icon = icons[s.IconName].Icon, Text = s.DisplayNumber, Tooltip = s.Tooltip,
                    OriginatorId = s.OriginatorId
                });
            ;

            AddStatusIconIfApplicable(statuses, TemporalStatType.Dodge, true, v => "Tooltips/DodgeAttacks".ToLocalized().SafeFormatWithDefault("Dodges the next {0} attacks", v.ToString()));
            AddStatusIconIfApplicable(statuses, StatType.Armor, true, v => "Tooltips/ReduceAttackDamage".ToLocalized().SafeFormatWithDefault("Reduces attack damage taken by {0}", v.ToString()));
            AddStatusIconIfApplicable(statuses, StatType.Resistance, true, v => "Tooltips/ReduceMagicDamage".ToLocalized().SafeFormatWithDefault("Reduces magic damage taken by {0}", v.ToString()));
            AddNegativeStatusIconIfApplicable(statuses, StatType.Armor, true, v => "Tooltips/IncreaseAttackDamage".ToLocalized().SafeFormatWithDefault("Increases attack damage taken by {0}", v.ToString()));
            AddNegativeStatusIconIfApplicable(statuses, StatType.Resistance, true, v => "Tooltips/IncreaseMagicDamage".ToLocalized().SafeFormatWithDefault("Increases magic damage taken by {0}", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.DoubleDamage, true, v => "Tooltips/DoubleDamage".ToLocalized().SafeFormatWithDefault("Double Damage for next {0} effects", v.ToString()));
            AddBuffAmountIconIfApplicable(statuses, StatType.Attack);
            AddBuffAmountIconIfApplicable(statuses, StatType.Magic);
            AddBuffAmountIconIfApplicable(statuses, StatType.Leadership);
            AddBuffAmountIconIfApplicable(statuses, StatType.Economy);
            AddStatusIconIfApplicable(statuses, TemporalStatType.Blind, true, v => "Tooltips/Blinded".ToLocalized().SafeFormatWithDefault("Blinded (guaranteed miss) for {0} Attacks", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Inhibit, true, v => "Tooltips/Inhibited".ToLocalized().SafeFormatWithDefault("Inhibited (guaranteed miss) for {0} Spells", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Taunt, true, v => "Tooltips/Taunt".ToLocalized().SafeFormatWithDefault("Taunt for {0} Turns", v.ToString()));
            if (_member.State[TemporalStatType.Stealth] > 0)
                statuses.Add(new CurrentStatusValue
                {
                    Type = TemporalStatType.Stealth.GetString(), Icon = icons[TemporalStatType.Stealth].Icon,
                    Tooltip = "Tooltips/Stealth".ToLocalized()
                });
            AddStatusIconIfApplicable(statuses, TemporalStatType.Disabled, true, v => "Tooltips/Disabled".ToLocalized().SafeFormatWithDefault("Disabled for {0} Turns", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Stun, true, v => "Tooltips/Stunned".ToLocalized().SafeFormatWithDefault("Stunned for {0} Cards. Reactions disabled.", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Confused, true, v => "Tooltips/Confused".ToLocalized().SafeFormatWithDefault("Confused for {0} Turns", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Aegis, true, v => "Tooltips/Aegis".ToLocalized().SafeFormatWithDefault("Prevents next {0} harmful effects", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Lifesteal, true, v => "Tooltips/Lifesteal".ToLocalized());
            AddStatusIconIfApplicable(statuses, TemporalStatType.Vulnerable, true, v => "Tooltips/Vulnerable".ToLocalized());
            AddStatusIconIfApplicable(statuses, TemporalStatType.AntiHeal, true, v => "Tooltips/AntiHeal".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.AfterShielded, () => "Tooltips/AfterShielded".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.OnClipUsed, () => "Tooltips/ClipUsedEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenBloodied, () => "Tooltips/BloodiedEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenShieldBroken, () => "Tooltips/ShieldBrokenEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.OnHpDamageDealt, () => "Tooltips/HpDamageDealtEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenAllyKilled, () => "Tooltips/AllyKilledEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenAfflicted, () => "Tooltips/AfflictedEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenIgnited, () => "Tooltips/IgnitedEffect".ToLocalized());
            AddStatusIconIfApplicable(statuses, TemporalStatType.Injury, true, v => "Tooltips/ReceivedInjuries".ToLocalized().SafeFormatWithDefault("Received {0} Injuries, applied at end of battle", v.ToString()));
            AddStatusIconIfApplicable(statuses, TemporalStatType.Marked, true, v => "Tooltips/Marked".ToLocalized());
            AddStatusIconIfApplicable(statuses, TemporalStatType.PreventResourceGains, true, v => "Tooltips/PreventResourceGains".ToLocalized().SafeFormatWithDefault("Cannot gain resources for {0} Turns", v.ToString()));

            var dotCombined = Combine(
                memberStatesByTag.ValueOrDefault(StatusTag.DamageOverTime, Array.Empty<ITemporalState>()),
                s => s.Status.CustomText.Select(t => t,
                    "Tooltips/DamageOverTime".ToLocalized().SafeFormatWithDefault("Takes {0} at the Start of the next {1} turns", s.Amount.Value, s.RemainingTurns.Value)),
                s => s.Sum(x => x.Amount.OrDefault(0)));
            dotCombined.IfPresent(d => statuses.Add(d));

            if (_member.State.HasStatus(StatusTag.HealOverTime))
                statuses.Add(new CurrentStatusValue
                {
                    Type = StatusTag.HealOverTime.GetString(), Icon = icons[StatusTag.HealOverTime].Icon,
                    Tooltip = "Tooltips/HealsTurnStart".ToLocalized()
                });

            if (_member.State[TemporalStatType.Prominent] > 0)
                statuses.Add(new CurrentStatusValue
                {
                    Type = TemporalStatType.Prominent.GetString(), Icon = icons[TemporalStatType.Prominent].Icon,
                    Tooltip = "Tooltips/CannotStealthProminent".ToLocalized()
                });

            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenHit, () => "Tooltips/OnHitEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenKilled, () => "Tooltips/WhenKilledEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.WhenDamaged, () => "Tooltips/WhenDamagedEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.StartOfTurnTrigger, () => "Tooltips/TurnStartEffect".ToLocalized());
            AddCustomTextStatusIcons(memberStatesByTag, statuses, StatusTag.EndOfTurnTrigger, () => "Tooltips/TurnEndEffect".ToLocalized());

            UpdateComparisonWithPrevious(statuses);
            UpdateStatuses(statuses);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private void AddCustomTextStatusIcons(Dictionary<StatusTag, ITemporalState[]> states, List<CurrentStatusValue> statuses, StatusTag statusTag, Func<string> getDefaultText)
    {
        try
        {
            var defaultText = getDefaultText();
            AddCustomTextStatusIcons(states, statuses, statusTag, defaultText);
        }
        catch (Exception e)
        {
            LogError(statusTag.ToString(), e);
            AddCustomTextStatusIcons(states, statuses, statusTag, "Developer Error - Unknown");
        }
    }

    private static void LogError(string status, Exception e)
    {
        Log.Error($"Status Bar - Unable to get default text for '{status}': {e.Message} - {e.StackTrace}");
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
            statuses.Add(new CurrentStatusValue { Type = stat.GetString(), Icon = icons[stat].Icon, Text = text, Tooltip = GetTooltipOrDeveloperError(stat.ToString(), makeTooltip, value)});
    }

    private static string GetTooltipOrDeveloperError(string stat, Func<float, string> makeTooltip, float value)
    {
        var tooltip = "Developer Error - Unknown";
        try
        {
            tooltip = makeTooltip(value);
        }
        catch (Exception e)
        {
            LogError(stat, e);
        }

        return tooltip;
    }

    private void AddStatusIconIfApplicable(List<CurrentStatusValue> statuses, StatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.GetCeilingIntString() : string.Empty;
        if (value > 0)
            statuses.Add(new CurrentStatusValue { Type = stat.GetString(), Icon = icons[stat].Icon, Text = text, Tooltip = GetTooltipOrDeveloperError(stat.ToString(), makeTooltip, value)});
    }

    private const string Negative = "-Negative";
    private void AddNegativeStatusIconIfApplicable(List<CurrentStatusValue> statuses, StatType stat, bool showNumber, Func<float, string> makeTooltip)
    {
        var value = _member.State[stat];
        var text = showNumber ? value.GetCeilingIntString() : string.Empty;
        if (value < 0)
            statuses.Add(new CurrentStatusValue { Type = stat.GetString(), Icon = icons[stat + Negative].Icon, Text = text, Tooltip = GetTooltipOrDeveloperError(stat.ToString(), makeTooltip, -value)});
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
