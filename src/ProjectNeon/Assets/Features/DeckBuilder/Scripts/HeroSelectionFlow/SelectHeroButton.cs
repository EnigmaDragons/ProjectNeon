using UnityEngine;
using UnityEngine.UI;

public class SelectHeroButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Image image;
    [SerializeField] private GameEvent deckSelectionRequired; 

    private Hero _hero;

    public void Init(Hero hero)
    {
        _hero = hero;
        image.sprite = _hero.Bust;
    }

    public void SelectHero()
    {
        state.SelectedHero = _hero;
        deckSelectionRequired.Publish();
    }
}
