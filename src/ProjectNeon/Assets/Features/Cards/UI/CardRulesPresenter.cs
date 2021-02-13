using System.Collections.Generic;
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
        if (d.TimingType == CardTimingType.Instant)
            rulesToShow.Add("Instant");
        if (d.Cost.PlusXCost)
            rulesToShow.Add("X-Cost");
        
        var battleEffects = d.BattleEffects();
        battleEffects.ForEach(b =>
        {
            if (b.EffectType == EffectType.ApplyVulnerable)
                rulesToShow.Add("Vulnerable");
            
            if (b.EffectScope == null) 
                return;
            if (b.EffectScope.Value.Equals("Evade"))
                rulesToShow.Add("Evade");
            if (b.EffectScope.Value.Equals("Taunt"))
                rulesToShow.Add("Taunt");
            if (b.EffectScope.Value.Equals("Blind"))
                rulesToShow.Add("Blind");
        });
        
        rulesToShow
            .ForEach(r => Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized(r));
    }

    public void Hide()
    {
        rulesParent.DestroyAllChildren();
    }
}
