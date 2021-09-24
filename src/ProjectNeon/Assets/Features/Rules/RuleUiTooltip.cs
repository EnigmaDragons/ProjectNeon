using UnityEngine;
using UnityEngine.EventSystems;

public class RuleUiTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private StringVariable key;
    [SerializeField] private StringKeyValueCollection rules;

    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(transform, rules[key], showBackground: true));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
    