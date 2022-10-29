using I2.Loc;
using UnityEngine;

public class CardKeywordRulePresenter : MonoBehaviour
{
    [SerializeField] private Localize textbox;
    [SerializeField] private StringKeyValueCollection keywordRules;

    public CardKeywordRulePresenter Initialized(string ruleKey)
    {
        if (keywordRules.Contains(ruleKey))
            textbox.SetTerm($"Keywords/{ruleKey}_Rule");
        else
            textbox.SetFinalText($"{Localized.String("Keywords", "Missing rule text for")} {Localized.String("Keywords", ruleKey)}");

        return this;
    }
}
