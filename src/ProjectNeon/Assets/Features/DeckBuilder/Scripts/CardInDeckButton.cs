using System.Linq;
using UnityEngine;

public class CardInDeckButton : OnMessage<DeckBuilderCurrentDeckChanged, SetSuperFocusDeckBuilderControl, StartBattleInitiated>
{
    [SerializeField] private SimpleDeckCardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameObject superFocus;
    
    private Maybe<Card> _card = Maybe<Card>.Missing();
    private CardType _cardType;
    private int _count;
    private bool _empty;
    
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => UpdateInfo();

    protected override void Execute(SetSuperFocusDeckBuilderControl msg)
    {
        if (msg.Name == DeckBuilderControls.CardInDeck)
            superFocus.SetActive(msg.Enabled);
    }

    public void Init(Card c, bool superFocusEnabled)
    {
        _empty = false;
        presenter.gameObject.SetActive(true);
        _card = c;
        _cardType = c.CardTypeOrNothing.Value;
        UpdateInfo();
        superFocus.SetActive(superFocusEnabled);
    }

    public void InitEmpty()
    {
        _empty = true;
        presenter.OnDeselect(null);
        presenter.gameObject.SetActive(false);
    }

    public void RemoveCard()
    {
        if (_empty)
            return;

        var deck = state.SelectedHeroesDeck.Deck;
        if (deck.NoneNonAlloc(x => x.Name == _cardType.Name)) return;
        
        deck.Remove(deck.First(x => x.Name == _cardType.Name));
        _count--;
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
        Message.Publish(new CardRemovedFromDeck(transform));
    }

    private void UpdateInfo()
    {
        if (_empty)
            return;
        _count = state.SelectedHeroesDeck.Deck.Count(x => x.Name == _cardType.Name);
        if (_card != null && _card.IsPresent)
            presenter.Initialized(_count, _card.Value);
        else
            presenter.Initialized(_count, _cardType);
        presenter.BindLeftClickAction(RemoveCard);
    }
    
    protected override void Execute(StartBattleInitiated msg)
        => presenter.DisableInteractions();
}
