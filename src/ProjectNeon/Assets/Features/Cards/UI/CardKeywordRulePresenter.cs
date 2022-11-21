using System.Linq;
using I2.Loc;
using UnityEngine;

public class CardKeywordRulePresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Localize textbox;
    [SerializeField] private StringKeyTermCollection keywordRules;

    public CardKeywordRulePresenter Initialized(string ruleKey)
    {
        if (keywordRules.Contains(ruleKey))
            textbox.SetTerm(keywordRules[ruleKey]);
        else
            textbox.SetFinalText($"{"Keywords/Missing rule text for".ToLocalized()} {$"Keywords/{ruleKey}".ToLocalized()}");

        return this;
    }

    public string[] GetLocalizeTerms()
        => keywordRules.All.Select(x => $"Keywords/{x.Key}").Concat("Keywords/Missing rule text for").ToArray();
}
