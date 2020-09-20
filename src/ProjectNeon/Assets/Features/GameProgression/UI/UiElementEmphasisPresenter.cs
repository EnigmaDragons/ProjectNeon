using UnityEngine;

public class UiElementEmphasisPresenter : OnMessage<UiEmphasisStateChanged>
{
    [SerializeField] private UiEmphasisState emphasis;
    [SerializeField] private string elementName;
    [SerializeField] private GameObject target;
    
    protected override void AfterEnable() => target.SetActive(emphasis.Contains(elementName));
    protected override void Execute(UiEmphasisStateChanged msg) => target.SetActive(emphasis.Contains(elementName));
}
