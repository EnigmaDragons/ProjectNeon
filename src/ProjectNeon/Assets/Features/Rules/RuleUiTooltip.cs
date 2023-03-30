using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RuleUiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms
{
    [SerializeField] private StringVariable key;
    [SerializeField] private StringKeyTermCollection rules;

    private bool _showing;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rules != null && key != null)
        {
            _showing = true;
            Message.Publish(new ShowTooltip(transform.position, rules[key].ToLocalized(), showBackground: true));
        }
    }

    private void Hide()
    {
        if (_showing)
        {
            Message.Publish(new HideTooltip());
            _showing = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData) => Hide();

    private void OnDisable() => Hide();
    
    private void OnDestroy() => Hide();

    public string[] GetLocalizeTerms()
        => new [] { rules[key] };
}
    