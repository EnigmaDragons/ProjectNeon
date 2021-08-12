using TMPro;
using UnityEngine;

public class CardZoneCountPresenter : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private GameObject deckVisual;
    [SerializeField] private bool showWithZeroCards = false;

    private void OnEnable()
    {
        zone.OnZoneCardsChanged.Subscribe(UpdateCardCount, this);
        UpdateCardCount();
    }

    private void OnDisable()
    {
        zone.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void UpdateCardCount()
    {
        var count = zone.Cards.Length;
        textField.text = count.ToString();
        textField.enabled = count > 0 || showWithZeroCards;
        deckVisual.SetActive(count > 0 || showWithZeroCards);
    }
}
