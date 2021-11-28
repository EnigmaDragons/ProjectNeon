using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatDiffPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statLabel;
    [SerializeField] private TextMeshProUGUI statBaseAmountLabel;
    [SerializeField] private Image diffIcon;
    [SerializeField] private TextMeshProUGUI diffAmountLabel;
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color neutralColor;

    private static readonly Color Transparent = new Color(0, 0, 0, 0);

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public StatDiffPresenter Initialized(string statName, int baseAmount, int diffAmount, bool showIfZero = false)
    {
        gameObject.SetActive(true);
        statLabel.text = ModifiedStatName(statName);
        statBaseAmountLabel.text = baseAmount.ToString().PadLeft(2, ' ');
        diffIcon.color = diffAmount > 0 ? positiveColor : Transparent;
        diffAmountLabel.text = diffAmount > 0 ? $"+{diffAmount}" : "";
        diffAmountLabel.color = diffAmount > 0 ? positiveColor : neutralColor;
        if (!showIfZero && baseAmount == 0 && diffAmount == 0)
            Hide();
        return this;
    }

    private string ModifiedStatName(string statName)
    {
        if (statName.Equals(StatType.StartingShield.ToString()))
            return "Shield";
        return statName;
    }
}
