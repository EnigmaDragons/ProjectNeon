using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionUI : MonoBehaviour
{
    private const float Padding = 10;

    [SerializeField] private PartyAdventureState party;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private SelectHeroButton[] selectHeroButtons;
    [SerializeField] private Button viewHeroDetailsButton;

    private void Awake() => viewHeroDetailsButton.onClick.AddListener(ViewHeroDetails);
    
    public void Init()
    {
        if (party.Heroes.None())
            return;
        
        Log.Info($"Hero Selection UI - Init - Heroes {string.Join(",", party.Heroes.Select(h => h.NameTerm.ToEnglish()))} Decks {party.Decks.Length}");
        var buttons = new List<RectTransform>();
        state.HeroesDecks = party.Decks.Select((deck, i) => new HeroesDeck { Deck = deck.Cards.ToList(), Hero = party.Heroes[i]}).ToList();
        for (var i = 0; i < 3; i++)
        {
            if (state.HeroesDecks.Count > i)
                selectHeroButtons[i].Init(state.HeroesDecks[i]);
            else
                selectHeroButtons[i].gameObject.SetActive(false);
        }
        for (var i = 0; i < buttons.Count; i++)
            buttons[i].anchoredPosition = new Vector2((i - (buttons.Count / 2f - 0.5f)) * (buttons[i].sizeDelta.x + Padding), buttons[i].anchoredPosition.y);
        SelectFirstHero();
    }

    private void SelectFirstHero()
    {
        if (state.HeroesDecks.Any())
            state.SelectedHeroesDeck = state.HeroesDecks.First();
    }

    private void ViewHeroDetails() => Message.Publish(new ShowHeroDetailsView(state.SelectedHero, Maybe<Member>.Missing()));
}