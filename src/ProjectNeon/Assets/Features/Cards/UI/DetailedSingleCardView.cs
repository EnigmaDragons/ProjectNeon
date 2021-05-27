using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DetailedSingleCardView : OnMessage<ShowDetailedCardView, HideDetailedCardView>
{
    [SerializeField] private GameObject viewParent;
    [SerializeField] private Image darken;
    [SerializeField] private CardPresenter card;

    private float _duration = 0.4f;
    private Vector3 _cardScale;
    private float _darkenAlpha;
    
    private void Awake()
    {
        _darkenAlpha = darken.color.a;
        _cardScale = card.transform.localScale;
        viewParent.SetActive(false);
    }

    protected override void Execute(ShowDetailedCardView msg)
    {
        msg.Card.ExecuteIfPresentOrElse(card.Set, () => card.Set(msg.CardType));
        card.ShowComprehensiveCardInfo();
        viewParent.SetActive(true);
        darken.color = new Color(darken.color.r, darken.color.g, darken.color.b, 0);
        darken.DOFade(_darkenAlpha, _duration);
        card.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        card.transform.DOScale(_cardScale, _duration);
    }

    protected override void Execute(HideDetailedCardView msg)
    {
        Message.Publish(new HideReferencedCard());
        viewParent.SetActive(false);
    }
}
