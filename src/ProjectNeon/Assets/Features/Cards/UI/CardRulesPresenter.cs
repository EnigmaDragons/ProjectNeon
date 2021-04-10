using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRulesPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;

    private bool _isInitialized;
    
    private void Awake()
    {
        if (!_isInitialized)
            Hide();
    }

    public void Show(CardTypeData d)
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
            rulesToShow.AddIf("Quick", d.Speed == CardSpeed.Quick);

            var battleEffects = d.BattleEffects().Concat(d.ReactionBattleEffects());
            battleEffects.ForEach(b =>
            {
                rulesToShow.AddIf("Vulnerable", b.EffectType == EffectType.ApplyVulnerable);
                rulesToShow.AddIf(TemporalStatType.Disabled.ToString(), b.EffectType == EffectType.DisableForTurns);
                rulesToShow.AddIf("Stealth", b.EffectType == EffectType.EnterStealth);
                rulesToShow.AddIf("Drain", b.EffectType == EffectType.DrainPrimaryResourceFormula);

                AddAllMatchingEffectScopeRules(rulesToShow, b,
                    TemporalStatType.Dodge.ToString(),
                    TemporalStatType.Evade.ToString(),
                    TemporalStatType.Taunt.ToString(),
                    TemporalStatType.Blind.ToString(),
                    TemporalStatType.Inhibit.ToString(),
                    TemporalStatType.Spellshield.ToString(),
                    TemporalStatType.CardStun.ToString(),
                    TemporalStatType.DoubleDamage.ToString(),
                    PlayerStatType.CardCycles.ToString(),
                    TemporalStatType.Disabled.ToString(),
                    TemporalStatType.Stealth.ToString(),
                    TemporalStatType.Confused.ToString());
            });

            rulesToShow
                .Distinct()
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
        _isInitialized = true;
        rulesParent.DestroyAllChildren();
    }
}
