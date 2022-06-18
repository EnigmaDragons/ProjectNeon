using System.Linq;
using TMPro;
using UnityEngine;

public class SimpleCardsUI : MonoBehaviour
{
    [SerializeField] private GameObject deckParent;
    [SerializeField] private CardInLibraryButton cardPrototype;
    [SerializeField] private TextMeshProUGUI headerLabel;
    
    public void Init(string header, Card[] cards)
    {
        headerLabel.text = header;
        deckParent.DestroyAllChildren();
        var sortedCards = cards
            .OrderBy(x => x.Owner.Id)
            .ThenBy(x => x.Cost.CostSortOrder())
            .ThenBy(x => x.GetArchetypeKey())
            .ThenBy(x => x.Name);
        foreach (var c in sortedCards)
            Instantiate(cardPrototype, deckParent.transform).InitInfoOnly(c, () => { });
    }
}
