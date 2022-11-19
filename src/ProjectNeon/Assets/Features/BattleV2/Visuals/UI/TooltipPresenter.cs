using I2.Loc;
using UnityEngine;

public sealed class TooltipPresenter : OnMessage<ShowTooltip, HideTooltip>
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Localize tooltipLabel;

    private void Awake() => panel.SetActive(false);
    
    protected override void Execute(ShowTooltip msg)
    {
        tooltipLabel.SetFinalText(msg.Text);
        if (!string.IsNullOrWhiteSpace(msg.Text))
            panel.SetActive(true);
    }

    protected override void Execute(HideTooltip msg)
    {
        panel.SetActive(false);
    }
}
