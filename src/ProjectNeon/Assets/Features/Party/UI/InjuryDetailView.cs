using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InjuryDetailView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI injuryNameLabel;
    [SerializeField] private TextMeshProUGUI injuryCountLabel;

    private string _tooltip;
    
    public void Init(int count, string injuryName, string tooltip)
    {
        injuryCountLabel.text = count.ToString();
        injuryNameLabel.text = injuryName;
        _tooltip = tooltip;
    }
    
    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(_tooltip));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
