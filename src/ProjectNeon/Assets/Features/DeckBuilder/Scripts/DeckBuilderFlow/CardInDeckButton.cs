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

    public string CardName;
    public int Count;

    public void Init(string cardName)
    {
        CardName = cardName;
        button.interactable = !state.TemporaryDeck.IsImmutable;
        UpdateInfo();
    }

    public void RemoveCard()
    {
        state.TemporaryDeck.Cards.Remove(state.TemporaryDeck.Cards.First(x => x.Name == CardName));
        Count--;
        deckChanged.Publish();
    }

    private void OnEnable() => deckChanged.Subscribe(UpdateInfo, this);
    private void OnDisable() => deckChanged.Unsubscribe(this);

    private void UpdateInfo()
    {
        Count = state.TemporaryDeck.Cards.Count(x => x.Name == CardName);
        cardNameText.text = CardName;
        countText.text = Count.ToString();
    }
}
