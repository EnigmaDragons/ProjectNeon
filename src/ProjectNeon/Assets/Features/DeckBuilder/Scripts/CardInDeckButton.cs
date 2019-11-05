using System.Linq;
using TMPro;
using UnityEngine;

public class CardInDeckButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent deckChanged;
    [SerializeField] private HoverCard hoverCard;

    private Canvas _canvas;
    private Card _card;
    private int _count;
    private GameObject _hoverCard;

    public void Init(Card card)
    {
        _card = card;
        UpdateInfo();
    }

    public void RemoveCard()
    {
        state.SelectedHeroesDeck.Deck.Remove(state.SelectedHeroesDeck.Deck.First(x => x.Name == _card.Name));
        _count--;
        deckChanged.Publish();
    }

    public void OnHover()
    {
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        _hoverCard.GetComponent<HoverCard>().Init(_card);
    }

    public void OnExit()
    {
        if (_hoverCard != null)
            Destroy(_hoverCard);
    }

    private void Awake() => _canvas = FindObjectOfType<Canvas>();
    private void OnEnable() => deckChanged.Subscribe(UpdateInfo, this);
    private void OnDisable() => deckChanged.Unsubscribe(this);
    private void OnDestroy() => OnExit();

    private void UpdateInfo()
    {
        _count = state.SelectedHeroesDeck.Deck.Count(x => x.Name == _card.Name);
        cardNameText.text = _card.Name;
        countText.text = _count.ToString();
    }
}
