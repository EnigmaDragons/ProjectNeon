using UnityEngine;

public class SingleCardZonePresenter : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private CardPresenter card;

    private void Awake()
    {
        card.gameObject.SetActive(false);
    }

    private void OnEnable() => zone.OnZoneCardsChanged.Subscribe(Render, this);
    private void OnDisable() => zone.OnZoneCardsChanged.Unsubscribe(this);

    private void Render()
    {
        card.gameObject.SetActive(false);
        if (zone.Count > 0)
            card.Set(zone.Cards[0]);
    }
}
