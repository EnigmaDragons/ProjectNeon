using System.Collections.Generic;
using UnityEngine;

public class ReferencedCardPresenter : OnMessage<ShowReferencedCard, HideReferencedCard>
{
    [SerializeField] private CardPresenter cardPrototype;

    private List<GameObject> _parents;
    
    private void Show(Card c, GameObject parent)
    {
        var cp = Instantiate(cardPrototype, parent.transform);
        cp.Set(c);
    }

    private void Show(CardTypeData c, GameObject parent)
    {
        var cp = Instantiate(cardPrototype, parent.transform);
        cp.Set(c);
    }

    public void Hide()
    {
        if (_parents != null)
        {
            foreach (var parent in _parents)
                parent.DestroyAllChildren();
            _parents = new List<GameObject>();
        }
    }

    protected override void Execute(ShowReferencedCard msg)
    {
        if (_parents == null)
            _parents = new List<GameObject>();
        _parents.Add(msg.Parent);
        msg.Card.ExecuteIfPresentOrElse(x => Show(x, msg.Parent), () => Show(msg.CardType, msg.Parent));
    }

    protected override void Execute(HideReferencedCard msg) => Hide();
}
