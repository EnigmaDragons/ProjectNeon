using TMPro;
using UnityEngine;

public class CardKeywordRulePresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textbox;
    [SerializeField] private StringKeyValueCollection keywordRules;

    public CardKeywordRulePresenter Initialized(string ruleKey)
    {
        textbox.text = keywordRules.ValueOrDefault(ruleKey, $"Missing Rule text for {ruleKey}");
        return this;
    }
}
