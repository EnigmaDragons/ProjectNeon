using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static KeywordRules;

public class GearRulesPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private GameObject alternateSideRulesPresenter;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;
    
    private bool _isInitialized;
    private bool _canUsePrimarySide;

    private void Awake()
    {
        if (_isInitialized) 
            return;
        
        Hide();
    }
    
    public void Show(Equipment e, int maxRulesToShow = 999)
    {
        if (e == null)
        {
            Log.Error($"Rules - Equipment is Null");
            return;
        }
        
        try
        {
            Hide();
            var rulesToShow = new List<string>();
            AddAllDescriptionFoundRules(rulesToShow, e.Description);

            var battleEffects = e.BattleStartEffects
                .Concat(e.BattleEndEffects)
                .Concat(e.TurnStartEffects)
                .Concat(e.TurnEndEffects);
            
            foreach (var b in battleEffects)
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
                .ForEach(r => Instantiate(rulePresenterPrototype, parent.transform).Initialized(r));
        }
        catch (Exception ex)
        {
            throw new Exception($"Equipment Rules Error for {e.Name}", ex);
        }
    }
    
    public void Hide()
    {
        _canUsePrimarySide = rulesParent.transform.position.x > 0;
        _isInitialized = true;
        rulesParent.DestroyAllChildren();
        alternateSideRulesPresenter.DestroyAllChildren();
    }
}
