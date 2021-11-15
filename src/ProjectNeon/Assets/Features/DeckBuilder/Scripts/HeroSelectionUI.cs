using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroSelectionUI : MonoBehaviour
{
    private const float Padding = 10;

    [SerializeField] private PartyAdventureState party;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private SelectHeroButton selectHeroButtonTemplate;
    [SerializeField] private Transform parent;

    public void Init()
    {
        Log.Info($"Hero Selection UI - Init - Heroes {string.Join(",", party.Heroes.Select(h => h.Name))} Decks {party.Decks.Length}");
        var buttons = new List<RectTransform>();
        state.HeroesDecks = party.Decks.Select((deck, i) => new HeroesDeck { Deck = deck.Cards.ToList(), Hero = party.Heroes[i]}).ToList();
        state.HeroesDecks.ForEach(x =>
        {
            var button = Instantiate(selectHeroButtonTemplate, parent);
            button.GetComponent<SelectHeroButton>().Init(x);
            buttons.Add(button.GetComponent<RectTransform>());
            Log.Info("Instantiate Hero Deck Button");
        });
        for (var i = 0; i < buttons.Count; i++)
            buttons[i].anchoredPosition = new Vector2((i - (buttons.Count / 2f - 0.5f)) * (buttons[i].sizeDelta.x + Padding), buttons[i].anchoredPosition.y);
        SelectFirstHero();
    }

    private void SelectFirstHero() => state.SelectedHeroesDeck = state.HeroesDecks.First();
}