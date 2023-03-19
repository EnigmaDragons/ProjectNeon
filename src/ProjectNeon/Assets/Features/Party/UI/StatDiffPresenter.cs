using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDiffPresenter : MonoBehaviour
{
    [SerializeField] private Localize statLabel;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI statBaseAmountLabel;
    [SerializeField] private Image diffIcon;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI diffAmountLabel;
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color neutralColor;

    private static readonly Color Transparent = new Color(0, 0, 0, 0);

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public StatDiffPresenter Initialized(StatType stat, int baseAmount, int diffAmount, float diffDelay, bool showIfZero = false)
    {
        gameObject.SetActive(true);
        if (stat == StatType.StartingShield)
            statLabel.SetTerm(TemporalStatType.Shield.GetTerm());
        else
            statLabel.SetTerm(stat.GetTerm());
        statBaseAmountLabel.text = baseAmount.ToString().PadLeft(2, ' ');
        diffIcon.color = Transparent;
        diffAmountLabel.text = "";
        
        if (!showIfZero && baseAmount == 0 && diffAmount == 0)
            Hide();
        else
            this.ExecuteAfterDelay(() => TriggerStatUpAnim(diffAmount), diffDelay);
        return this;
    }

    private void TriggerStatUpAnim(int diffAmount)
    {
        diffIcon.color = diffAmount > 0 ? positiveColor : Transparent;
        diffIcon.transform.DOPunchScaleStandard(new Vector3(1, 1, 1));
        diffAmountLabel.text = diffAmount > 0 ? $"+{diffAmount}" : "";
        diffAmountLabel.color = diffAmount > 0 ? positiveColor : neutralColor;
        diffAmountLabel.transform.DOPunchScaleStandard(new Vector3(1, 1, 1));
        if (diffAmount > 0)
            Message.Publish(new StatLeveledUp(transform));
    }
}
