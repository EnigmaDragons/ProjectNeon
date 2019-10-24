using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Image image;

    private Hero _hero;
    private DeckBuilderNavigation _navigation;

    public void Init(Hero hero, DeckBuilderNavigation navigation)
    {
        _hero = hero;
        _navigation = navigation;
        image.sprite = _hero.Bust;
    }

    public void SelectHero()
    {
        state.SelectedHero = _hero;
        _navigation.NavigateToDeckSelection();
    }
}
