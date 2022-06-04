using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static KeywordRules;

public class TwoSidedRulesDescriptionPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private GameObject alternateSideRulesPresenter;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;

    private int _shownRulesCount;
    private bool _isInitialized;
    private bool _canUsePrimarySide;

    public int ShownRulesCount => _shownRulesCount;
    
    private void Awake()
    {
        if (_isInitialized) 
            return;
        
        Hide();
    }
    
    public void Show(RulePanelContext ctx, int maxRulesToShow = 999)
    {
        if (ctx == null || ctx.Description == null || ctx.Effects == null)
        {
            Log.Error($"Rules - An Input is Null");
            return;
        }
        
        try
        {
            Hide();
            var rulesToShow = new List<string>();
            AddAllDescriptionFoundRules(rulesToShow, ctx.Description);
            
            foreach (var b in ctx.Effects)
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
            }

            var parent = _canUsePrimarySide ? rulesParent : alternateSideRulesPresenter;
            rulesToShow
                .Distinct()
                .OrderBy(ImportanceOrder)
                .Take(maxRulesToShow)
                .ForEach(r =>
                {
                    _shownRulesCount++;
                    Instantiate(rulePresenterPrototype, parent.transform).Initialized(r);
                });
        }
        catch (Exception ex)
        {
            throw new Exception($"Rules Error for {ctx.SourceName}", ex);
        }
    }
    
    public void Hide()
    {
        _shownRulesCount = 0;
        _canUsePrimarySide = rulesParent.transform.position.x > 0;
        _isInitialized = true;
        rulesParent.DestroyAllChildren();
        alternateSideRulesPresenter.DestroyAllChildren();
    }
}
