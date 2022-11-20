using UnityEngine;

public class TickerPresenter : MonoBehaviour
{
    [SerializeField] private TickerItem newsItemPrototype;
    [SerializeField] private AllTickerEntries entries;
    [SerializeField] private GameObject tickerParent;

    private IndexSelector<TickerEntry> _feed;
    private IndexSelector<TickerItem> _items;
    
    private void Awake()
    {
        _feed = new IndexSelector<TickerEntry>(entries.Entries.Shuffled());
        
        tickerParent.DestroyAllChildrenImmediate();
        _items = new IndexSelector<TickerItem>(new[]
        {
            Instantiate(newsItemPrototype, tickerParent.transform).Hidden(),
            Instantiate(newsItemPrototype, tickerParent.transform).Hidden()
        });
    }

    private void Start()
    {
        ShowNext();
    }
    
    private void ShowNext()
    {
        _items.MoveNext().Initialized(_feed.Current.Term, ShowNext);
        _feed.MoveNext();
    }
}
