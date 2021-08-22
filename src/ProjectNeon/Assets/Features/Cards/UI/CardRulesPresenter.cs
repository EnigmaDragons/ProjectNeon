using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRulesPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;

    private bool _isInitialized;
    
    // Rule Importance
    private static readonly DictionaryWithDefault<string, int> RulesByImportance = new DictionaryWithDefault<string, int>(0);
    
    private static readonly string[] RulesByImportanceArr = {
        "Drain",
        "Buyout",
        "Afflicted",
        "Igniting",
        "Bloodied",
        "Sneaky",
        "Profitable",
        "Defenseless",
        TemporalStatType.Marked.ToString(),
        TemporalStatType.DoubleDamage.ToString(),
        TemporalStatType.Stun.ToString(),
        TemporalStatType.Aegis.ToString(),
        TemporalStatType.Blind.ToString(),
        TemporalStatType.Inhibit.ToString(),
        TemporalStatType.Marked.ToString(),
        TemporalStatType.Dodge.ToString(),
        TemporalStatType.Taunt.ToString(),
        TemporalStatType.Disabled.ToString(),
        TemporalStatType.Stealth.ToString(),
        TemporalStatType.Lifesteal.ToString(),
        TemporalStatType.Confused.ToString(),
        ReactionConditionType.OnSlay.ToString(),
        "Glitch",
        "TagPlayed",
        TemporalStatType.Vulnerable.ToString(),
        "Critical",
        "PrimaryStat",
        PlayerStatType.CardCycles.ToString(),
        "Swap",
        "SingleUse",
        "X-Cost",
        "Chain",
        "Quick"
    };

    private void Awake()
    {
        if (_isInitialized) 
            return;
        
        InitIfNeeded();
        Hide();
    }

    private void InitIfNeeded()
    {
        if (_isInitialized)
            return;
        
        _isInitialized = true;
        if (RulesByImportance.Count != RulesByImportanceArr.Length)
        {
            RulesByImportance.Clear();
            RulesByImportanceArr.ForEachIndex((r, i) => RulesByImportance[r] = i);
        }
    }

    public void Show(Card c, int maxRulesToShow = 999)
        => Show(c.Type, maxRulesToShow, c.Mode == CardMode.Glitched);
    
    public void Show(CardTypeData d, int maxRulesToShow = 999, bool isGlitched = false)
    {
        if (d == null)
        {
            Log.Error($"Rules - Card Type Data is Null");
            return;
        }
        
        try
        {
            Hide();
            var rulesToShow = new List<string>();
            rulesToShow.AddIf("X-Cost", d.Cost.PlusXCost);
            rulesToShow.AddIf("Chain", d.ChainedCard.IsPresent);
            rulesToShow.AddIf("Swap", d.SwappedCard.IsPresent);
            rulesToShow.AddIf("Quick", d.Speed == CardSpeed.Quick);
            rulesToShow.AddIf("Afflicted", d.Conditions().Any(x => x.ConditionType == ActionConditionType.TargetHasDamageOverTime) 
                                           || d.Description.IndexOf("Afflict", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("Bloodied", d.Description.IndexOf("Bloodied", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("Bloodied", d.Description.IndexOf("Bloody", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("Sneaky", d.Description.IndexOf("Sneaky", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("Profitable", d.Description.IndexOf("profitable", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("Defenseless", d.Description.IndexOf("Defenseless", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("SingleUse", d.IsSinglePlay);
            rulesToShow.AddIf("Buyout", d.Description.IndexOf("Buyout", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf("Critical", d.Description.IndexOf("Crit", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf(TemporalStatType.Marked.ToString(), d.Description.IndexOf("Mark", StringComparison.OrdinalIgnoreCase) >= 0);
            rulesToShow.AddIf(TemporalStatType.Lifesteal.ToString(), d.Description.IndexOf(TemporalStatType.Lifesteal.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);

            var battleEffects = d.BattleEffects().Concat(d.ReactionBattleEffects());
            battleEffects.ForEach(b =>
            {
                rulesToShow.AddIf("TagPlayed", b.Formula.Contains("Tag["));
                rulesToShow.AddIf(TemporalStatType.Disabled.ToString(), b.EffectType == EffectType.DisableForTurns);
                rulesToShow.AddIf(TemporalStatType.Stealth.ToString(), b.EffectType == EffectType.EnterStealth);
                rulesToShow.AddIf("Drain", b.EffectType == EffectType.TransferPrimaryResourceFormula);
                rulesToShow.AddIf("Igniting", "Igniting".Equals(b.ReactionEffectScope.Value));
                rulesToShow.AddIf(ReactionConditionType.OnSlay.ToString(), ReactionConditionType.OnSlay == b.ReactionConditionType);
                rulesToShow.AddIf("Glitch", b.EffectType == EffectType.GlitchRandomCards);

                AddAllMatchingEffectScopeRules(rulesToShow, b,
                    TemporalStatType.Dodge.ToString(),
                    TemporalStatType.Taunt.ToString(),
                    TemporalStatType.Blind.ToString(),
                    TemporalStatType.Inhibit.ToString(),
                    TemporalStatType.Aegis.ToString(),
                    TemporalStatType.Stun.ToString(),
                    TemporalStatType.DoubleDamage.ToString(),
                    PlayerStatType.CardCycles.ToString(),
                    TemporalStatType.Disabled.ToString(),
                    TemporalStatType.Stealth.ToString(),
                    TemporalStatType.Lifesteal.ToString(),
                    TemporalStatType.Confused.ToString(),
                    TemporalStatType.Marked.ToString(),
                    TemporalStatType.Vulnerable.ToString(),
                    "PrimaryStat");
            });

            // Overwhelming Card State Rules
            if (isGlitched)
                rulesToShow = new List<string> { "Glitch" };
            
            rulesToShow
                .Distinct()
                .OrderBy(r => RulesByImportance[r])
                .Take(maxRulesToShow)
                .ForEach(r => Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized(r));
        }
        catch (Exception e)
        {
            throw new Exception($"Card Rules Error for {d.Name}", e);
        }
    }
    
    private void AddAllMatchingEffectScopeRules(List<string> rulesToShow, EffectData e, params string[] scopes) 
        => scopes.ForEach(s => rulesToShow.AddIf(s, e.EffectScope != null && s.Equals(e.EffectScope.Value)));
    
    public void Hide()
    {
        InitIfNeeded();
        rulesParent.DestroyAllChildren();
    }
}
