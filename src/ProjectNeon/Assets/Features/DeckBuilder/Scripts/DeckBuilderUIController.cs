using UnityEngine;

public class DeckBuilderUIController : OnMessage<ShowDeckBuilder, CloseDeckBuilder>
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);

    protected override void Execute(ShowDeckBuilder msg) => target.SetActive(true);
    protected override void Execute(CloseDeckBuilder msg) => target.SetActive(false);
}
