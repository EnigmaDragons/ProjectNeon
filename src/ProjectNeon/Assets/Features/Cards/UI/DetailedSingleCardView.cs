using UnityEngine;
using UnityEngine.UI;

public class DetailedSingleCardView : OnMessage<ShowDetailedCardView, HideDetailedCardView>
{
    [SerializeField] private GameObject viewParent;
    [SerializeField] private Image darken;
    [SerializeField] private CardPresenter card;

    private void Awake() => viewParent.SetActive(false);
    
    protected override void Execute(ShowDetailedCardView msg)
    {
        Log.Info("Received ShowDetailed Card View Message");
        msg.Card.ExecuteIfPresentOrElse(card.Set, () => card.Set(msg.CardType));
        card.ShowComprehensiveCardInfo();
        viewParent.SetActive(true);
    }

    protected override void Execute(HideDetailedCardView msg)
    {
        viewParent.SetActive(false);
    }
}
