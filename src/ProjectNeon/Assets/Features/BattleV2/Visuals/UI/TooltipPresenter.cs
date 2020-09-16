using TMPro;
using UnityEngine;

public sealed class TooltipPresenter : OnMessage<ShowTooltip, HideTooltip>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI tooltipLabel;

    private void Awake() => panel.SetActive(false);
    
    protected override void Execute(ShowTooltip msg)
    {
        tooltipLabel.text = msg.Text;
        if (!string.IsNullOrWhiteSpace(msg.Text))
            panel.SetActive(true);
    }

    protected override void Execute(HideTooltip msg)
    {
        panel.SetActive(false);
    }
}
