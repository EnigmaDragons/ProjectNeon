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
        
        var battleEffects = d.BattleEffects();
        battleEffects.ForEach(b =>
        {
            if (b.EffectScope == null) 
                return;
            if (b.EffectScope.Value.Equals("Evade"))
                rulesToShow.Add("Evade");
            if (b.EffectScope.Value.Equals("Taunt"))
                rulesToShow.Add("Taunt");
        });
        
        rulesToShow
            .ForEach(r => Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized(r));
    }

    public void Hide()
    {
        rulesParent.DestroyAllChildren();
    }
}
