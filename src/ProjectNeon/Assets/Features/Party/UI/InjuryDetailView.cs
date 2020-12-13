using TMPro;
using UnityEngine;

public class InjuryDetailView : MonoBehaviour
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

    public void OnMouseEnter() => Message.Publish(new ShowTooltip(_tooltip));
    public void OnMouseExit() => Message.Publish(new HideTooltip());
}
