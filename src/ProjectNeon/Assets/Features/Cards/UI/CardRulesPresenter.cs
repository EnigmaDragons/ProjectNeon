using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static KeywordRules;

public class CardRulesPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;

    private bool _isInitialized;

    private void Awake()
    {
        if (_isInitialized) 
            return;
        
        Hide();
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
            rulesToShow.AddIf(Chain, d.ChainedCard.IsPresent);
            rulesToShow.AddIf("Swap", d.SwappedCard.IsPresent);
            rulesToShow.AddIf("Quick", d.Speed == CardSpeed.Quick);
            rulesToShow.AddIf("SingleUse", d.IsSinglePlay);
            rulesToShow.AddIf("Afflicted", d.Conditions().Any(x => x.ConditionType == ActionConditionType.TargetHasDamageOverTime));
            
            AddAllDescriptionFoundRules(rulesToShow, d.Description);
            
            var battleEffects = d.BattleEffects().Concat(d.ReactionBattleEffects());
            
            foreach (var b in battleEffects)
            {
                rulesToShow.AddIf("TagPlayed", b.Formula.Contains("Tag["));
                rulesToShow.AddIf(TemporalStatType.Disabled.ToString(), b.EffectType == EffectType.DisableForTurns);
                rulesToShow.AddIf(TemporalStatType.Stealth.ToString(), b.EffectType == EffectType.EnterStealth);
                rulesToShow.AddIf(Drain, b.EffectType == EffectType.TransferPrimaryResourceFormula);
                rulesToShow.AddIf(Igniting, Igniting.Equals(b.ReactionEffectScope.Value));
                rulesToShow.AddIf(ReactionConditionType.OnSlay.ToString(), ReactionConditionType.OnSlay == b.ReactionConditionType);
                rulesToShow.AddIf(Glitch, b.EffectType == EffectType.GlitchRandomCards);

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
                    PrimaryStat);
            }

            // Overwhelming Card State Rules
            if (isGlitched)
                rulesToShow = new List<string> { Glitch };
            
            rulesToShow
                .Distinct()
                .OrderBy(ImportanceOrder)
                .Take(maxRulesToShow)
                .ForEach(r => Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized(r));
        }
        catch (Exception e)
        {
            throw new Exception($"Card Rules Error for {d.Name}", e);
        }
    }
    
    public void Hide()
    {
        _isInitialized = true;
        rulesParent.DestroyAllChildren();
    }
}
