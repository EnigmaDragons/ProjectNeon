
using UnityEngine;

public class ShowOnAnyInvalidDeck : OnMessage<DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private GameObject target;
    [SerializeField] private DeckBuilderState state;

    protected override void AfterEnable() => Render();

    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => Render();

    private void Render() => target.SetActive(!state.AllHeroDecksAreValid);
}
