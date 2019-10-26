using System.Linq;
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

    private string _cardName;
    private int _count;

    public void Init(string cardName)
    {
        _cardName = cardName;
        button.interactable = !state.TemporaryDeck.IsImmutable;
        UpdateInfo();
    }

    public void RemoveCard()
    {
        state.TemporaryDeck.Cards.Remove(state.TemporaryDeck.Cards.First(x => x.Name == _cardName));
        _count--;
        deckChanged.Publish();
    }

    private void OnEnable() => deckChanged.Subscribe(UpdateInfo, this);
    private void OnDisable() => deckChanged.Unsubscribe(this);

    private void UpdateInfo()
    {
        _count = state.TemporaryDeck.Cards.Count(x => x.Name == _cardName);
        cardNameText.text = _cardName;
        countText.text = _count.ToString();
    }
}
