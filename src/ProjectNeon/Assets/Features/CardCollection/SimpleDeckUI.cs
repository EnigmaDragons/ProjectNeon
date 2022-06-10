using System.Linq;
using UnityEngine;

public class SimpleDeckUI : MonoBehaviour
{
    [SerializeField] private GameObject deckParent;
    [SerializeField] private SimpleDeckCardPresenter cardPrototype;

    private int CostSortOrder(IResourceAmount cost) => cost.PlusXCost ? 99 : cost.BaseAmount;

    public void Init(Card[] cards, bool showCount = true)
    {
        deckParent.DestroyAllChildren();
        var canvas = gameObject.transform.GetComponentInParent<Canvas>();
        foreach (var c in cards.OrderBy(x => CostSortOrder(x.Cost)).ThenBy(x => x.GetArchetypeKey()).GroupBy(x => x.Id))
            Instantiate(cardPrototype, deckParent.transform).Initialized(showCount ? c.Count() : -1, c.First())
                .SetCanvas(canvas);
    }
}
