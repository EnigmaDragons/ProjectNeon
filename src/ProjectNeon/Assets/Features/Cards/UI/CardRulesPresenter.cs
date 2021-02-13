using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRulesPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;

    private void Awake() => Hide();

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
            
            AddAllMatchingEffectScopeRules(rulesToShow, b, 
                TemporalStatType.Evade.ToString(), 
                TemporalStatType.Taunt.ToString(), 
                TemporalStatType.Blind.ToString(),
                TemporalStatType.Spellshield.ToString(),
                TemporalStatType.CardStun.ToString(), 
                PlayerStatType.CardCycles.ToString());
        });
        
        rulesToShow
            .ForEach(r => Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized(r));
    }

    private void AddAllMatchingEffectScopeRules(List<string> rulesToShow, EffectData e, params string[] scopes) 
        => scopes.ForEach(s => rulesToShow.AddIf(s, e.EffectScope != null && s.Equals(e.EffectScope.Value)));

    public void Hide()
    {
        rulesParent.DestroyAllChildren();
    }
}
