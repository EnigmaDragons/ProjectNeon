using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButton : OnMessage<SetSuperFocusDeckBuilderControl>
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private IntReference DeckSize;
    [SerializeField] private Image toTint;
    [SerializeField] private Image image;
    [SerializeField] private Image selected;
    [SerializeField] private GameObject superFocus;

    private HeroesDeck _heroesDeck;

    public void Init(HeroesDeck heroesDeck)
    {
        gameObject.SetActive(true);
        _heroesDeck = heroesDeck;
        image.sprite = _heroesDeck.Hero.Character.Bust;
    }

    public void SelectHero()
    {
        state.SelectedHeroesDeck = _heroesDeck;
    }

    private void Update()
    {
        if (_heroesDeck == null || toTint == null || image == null || selected == null)
            return;
        
        selected.gameObject.SetActive(state.SelectedHero == _heroesDeck.Hero);
        toTint.color = _heroesDeck.Deck.Count == DeckSize.Value ? Color.white : Color.red;
    }

    protected override void Execute(SetSuperFocusDeckBuilderControl msg)
    {
        if (msg.Name == DeckBuilderControls.HeroTab)
            superFocus.SetActive(msg.Enabled && state.SelectedHeroesDeck != _heroesDeck);
    }
}
