using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInDeckButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button button;
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
        button.interactable = !state.TemporaryDeck.IsImmutable;
        UpdateInfo();
    }

    public void RemoveCard()
    {
        state.TemporaryDeck.Cards.Remove(state.TemporaryDeck.Cards.First(x => x.Name == _card.Name));
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

    private void UpdateInfo()
    {
        _count = state.TemporaryDeck.Cards.Count(x => x.Name == _card.Name);
        cardNameText.text = _card.Name;
        countText.text = _count.ToString();
    }
}
