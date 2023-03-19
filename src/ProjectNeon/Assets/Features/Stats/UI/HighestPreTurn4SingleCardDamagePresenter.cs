using TMPro;
using UnityEngine;

public class HighestPreTurn4SingleCardDamagePresenter : MonoBehaviour
{
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI label;

    private void OnEnable()
    {
        label.text = PermanentStats.Data.HighestPreTurn4SingleCardDamage.ToString();
    }
}
