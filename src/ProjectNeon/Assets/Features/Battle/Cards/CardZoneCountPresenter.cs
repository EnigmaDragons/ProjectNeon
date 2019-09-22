using TMPro;
using UnityEngine;

public class CardZoneCountPresenter : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private TextMeshProUGUI textField;

    private void OnEnable()
    {
        zone.OnZoneCardsChanged.Subscribe(UpdateCardCount, this);
    }

    private void OnDisable()
    {
        zone.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void UpdateCardCount() => textField.text = zone.Cards.Length.ToString();
}
