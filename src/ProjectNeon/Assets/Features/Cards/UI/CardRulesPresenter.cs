using UnityEngine;

public class CardRulesPresenter : MonoBehaviour
{
    [SerializeField] private GameObject rulesParent;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;

    private void Awake() => Hide();

    public void Show(CardTypeData d)
    {
        Hide();
        if (d.TimingType == CardTimingType.Instant)
            Instantiate(rulePresenterPrototype, rulesParent.transform).Initialized("Instant");
    }

    public void Hide()
    {
        rulesParent.DestroyAllChildren();
    }
}
