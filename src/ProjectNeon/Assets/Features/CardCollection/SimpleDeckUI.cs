using System.Linq;
using UnityEngine;

public class SimpleDeckUI : MonoBehaviour
{
    [SerializeField] private GameObject deckParent;
    [SerializeField] private SimpleDeckCardPresenter cardPrototype;
    
    public void Init(Card[] cards, bool showCount = true)
    {
        deckParent.DestroyAllChildren();
        foreach (var c in cards.GroupBy(x => x.Id))
            Instantiate(cardPrototype, deckParent.transform).Init(showCount ? c.Count() : -1, c.First());
    }
}
