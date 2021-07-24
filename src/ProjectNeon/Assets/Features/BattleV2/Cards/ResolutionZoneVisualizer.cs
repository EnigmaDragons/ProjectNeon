using System.Linq;
using UnityEngine;

public class ResolutionZoneVisualizer : MonoBehaviour
{
    [SerializeField] private CardPlayZone zone;
    [SerializeField] private CardPresenter cardProto;
    [SerializeField] private Vector3 startingScale = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 animScaleAmount = new Vector3(0.8f, 0.8f, 0.8f);
    
    private bool _isDirty = true;
    private Maybe<Card> _card;
    private CardPresenter _lastCard;
    
    void OnEnable()
    {
        _isDirty = true;
        zone.OnZoneCardsChanged.Subscribe(new GameEventSubscription(zone.OnZoneCardsChanged.name, x => _isDirty = true, this));
    }

    void OnDisable()
    {
        zone.OnZoneCardsChanged.Unsubscribe(this);
    }

    void Update()
    {
        if (!_isDirty)
            return;

        _isDirty = false;
        UpdateZone();
    }
    
    void UpdateZone()
    {
        if (zone.Cards.None())
        {
            Log.Info("Resolution Zone Update. No Resolution Zone cards.");
            _card = Maybe<Card>.Missing();
            Clear();
        }
        else if (_card.Select(c => c.CardId, -1) != zone.Cards.First().CardId)
        {
            Log.Info("Resolution Zone Update. Show new Resolution Zone card.");
            Show(zone.Cards.First());
        }
    }
    
    void Show(Card c)
    {
        Clear();
        
        _lastCard = Instantiate(cardProto, transform);
        _lastCard.Set(c);
        _lastCard.transform.localScale = startingScale;
        Message.Publish(new TweenMovementRequested(_lastCard.transform, animScaleAmount, 0.6f, MovementDimension.Scale, TweenMovementType.GoTo, "Resolving Card"));
    }
    
    private void Clear()
    {
        if (_lastCard != null)
            DestroyImmediate(_lastCard.gameObject);
    }
}
