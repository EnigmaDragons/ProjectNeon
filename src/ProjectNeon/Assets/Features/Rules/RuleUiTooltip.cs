using UnityEngine;
using UnityEngine.EventSystems;

public class RuleUiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ILocalizeTerms
{
    [SerializeField] private StringVariable key;
    [SerializeField] private StringKeyTermCollection rules;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rules != null && key != null)
            Message.Publish(new ShowTooltip(transform, rules[key].ToLocalized(), showBackground: true));
    }

    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());

    public string[] GetLocalizeTerms()
        => new [] { rules[key] };
}
    