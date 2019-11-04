using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private IntReference DeckSize;
    [SerializeField] private Image toTint;
    [SerializeField] private Image image;
    [SerializeField] private Image selected;

    private HeroesDeck _heroesDeck;

    public void Init(HeroesDeck heroesDeck)
    {
        _heroesDeck = heroesDeck;
        image.sprite = _heroesDeck.Hero.Bust;
    }

    public void SelectHero()
    {
        state.SelectedHeroesDeck = _heroesDeck;
    }

    private void Update()
    {
        selected.gameObject.SetActive(state.SelectedHeroesDeck == _heroesDeck);
        toTint.color = _heroesDeck.Deck.Count == DeckSize.Value ? Color.white : Color.red;
    } 
}
