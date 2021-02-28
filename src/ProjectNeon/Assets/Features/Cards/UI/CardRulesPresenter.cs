using System.Collections.Generic;
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
        Hide();
        var rulesToShow = new List<string>();
        rulesToShow.AddIf("Instant", d.TimingType == CardTimingType.Instant);
        rulesToShow.AddIf("X-Cost", d.Cost.PlusXCost);
        rulesToShow.AddIf("Chain", d.ChainedCard.IsPresent);
        
        var battleEffects = d.BattleEffects();
        battleEffects.ForEach(b =>
        {
            rulesToShow.AddIf("Vulnerable", b.EffectType == EffectType.ApplyVulnerable);
            rulesToShow.AddIf(TemporalStatType.Disabled.ToString(), b.EffectType == EffectType.DisableForTurns);
            rulesToShow.AddIf("Stealth", b.EffectType == EffectType.EnterStealth);
            
            AddAllMatchingEffectScopeRules(rulesToShow, b, 
                TemporalStatType.Evade.ToString(), 
                TemporalStatType.Taunt.ToString(), 
                TemporalStatType.Blind.ToString(),
                TemporalStatType.Spellshield.ToString(),
                TemporalStatType.CardStun.ToString(),
                TemporalStatType.DoubleDamage.ToString(),
                PlayerStatType.CardCycles.ToString(),
                TemporalStatType.Disabled.ToString());
        });
        
        rulesToShow
            .ForEach(r => Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized(r));
    }

    private void AddAllMatchingEffectScopeRules(List<string> rulesToShow, EffectData e, params string[] scopes) 
        => scopes.ForEach(s => rulesToShow.AddIf(s, e.EffectScope != null && s.Equals(e.EffectScope.Value)));

    public void Hide()
    {
        _isInitialized = true;
        rulesParent.DestroyAllChildren();
    }
}
