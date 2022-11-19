using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InjuryDetailView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Localize injuryNameLabel;
    [SerializeField, NoLocalizationNeededAttribute] private TextMeshProUGUI injuryCountLabel;

    private string _tooltip;
    
    public void Init(int count, string injuryName, string tooltip)
    {
        injuryCountLabel.text = count.ToString();
        injuryNameLabel.SetTerm($"Tooltips/Injury_{injuryName}");
        _tooltip = tooltip;
    }
    
    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(transform, _tooltip));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}
