using UnityEngine;

public class ReferencedCardPresenter : OnMessage<ShowReferencedCard, HideReferencedCard>
{
    [SerializeField] private CardPresenter cardPrototype;

    private GameObject _parent;
    
    private void Show(Card c)
    {
        var cp = Instantiate(cardPrototype, _parent.transform);
        cp.Set(c);
        
    }

    private void Show(CardTypeData c)
    {
        var cp = Instantiate(cardPrototype, _parent.transform);
        cp.Set(c);
    }

    public void Hide()
    {
        if (_parent != null)
            _parent.DestroyAllChildren();
    }

    protected override void Execute(ShowReferencedCard msg)
    {
        _parent = msg.Parent;
        msg.Card.ExecuteIfPresentOrElse(Show, () => Show(msg.CardType));
    }

    protected override void Execute(HideReferencedCard msg) => Hide();
}
