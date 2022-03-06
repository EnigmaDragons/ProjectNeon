using System;
using System.Linq;
using UnityEngine;

public class FullGlossaryPresenter : MonoBehaviour
{
    [SerializeField] private StringKeyValueCollection keywordRules;
    [SerializeField] private CardKeywordRulePresenter rulePresenterPrototype;
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private GameObject emptyObj;
    
    private void Awake() => GenerateLibrary();

    private void GenerateLibrary()
    {
        pageViewer.Init(
            rulePresenterPrototype.gameObject, 
            emptyObj, 
            keywordRules.All.OrderBy(k => k.Key.Value).Select(InitElement).ToList(),
            x => {},
            false);
    }

    private Action<GameObject> InitElement(StringKeyValuePair k)
        => gameObj => gameObj.GetComponent<CardKeywordRulePresenter>().Initialized(k.Key);
}
