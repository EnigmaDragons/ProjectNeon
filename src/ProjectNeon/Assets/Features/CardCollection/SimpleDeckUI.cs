using System.Linq;
using UnityEngine;

public class SimpleDeckUI : MonoBehaviour
{
    [SerializeField] private GameObject deckParent;
    [SerializeField] private SimpleDeckCardPresenter cardPrototype;
    [SerializeField] private Canvas canvasOverride;

    public void Init(Card[] cards, bool showCount = true)
    {
        deckParent.DestroyAllChildren();
        var canvas = canvasOverride != null 
            ? canvasOverride 
            : gameObject.transform.GetComponentInParent<Canvas>();
        
        foreach (var c in cards
            .OrderBy(x => x.IsBasic ? -999 : 0)
            .ThenBy(x => x.Cost.CostSortOrder())
            .ThenBy(x => x.GetArchetypeKey())
            .GroupBy(x => x.Id))
                Instantiate(cardPrototype, deckParent.transform).Initialized(showCount ? c.Count() : -1, c.First())
                    .SetCanvas(canvas);
    }
}
