using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroSelectionUI : MonoBehaviour
{
    private const float Padding = 10;

    [SerializeField] private Party party;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private SelectHeroButton selectHeroButtonTemplate;
    [SerializeField] private Transform parent;

    private void Start()
    {
        var buttons = new List<RectTransform>();
        state.HeroesDecks = party.Decks.Select((deck, i) => new HeroesDeck { Deck = deck.Cards.ToList(), Hero = party.Heroes[i]}).ToList();
        state.HeroesDecks.ForEach(x =>
        {
            var button = Instantiate(selectHeroButtonTemplate, parent);
            button.GetComponent<SelectHeroButton>().Init(x);
            buttons.Add(button.GetComponent<RectTransform>());
        });
        for (var i = 0; i < buttons.Count; i++)
            buttons[i].anchoredPosition = new Vector2((i - (buttons.Count / 2f - 0.5f)) * (buttons[i].sizeDelta.x + Padding), buttons[i].anchoredPosition.y);
        state.SelectedHeroesDeck = state.HeroesDecks.First();
    }
}
