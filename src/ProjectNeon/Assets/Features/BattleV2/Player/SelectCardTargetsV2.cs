using System.Linq;
using UnityEngine;

public class SelectCardTargetsV2 : MonoBehaviour, IConfirmCancellable
{
    [SerializeField] private CardResolutionZone cardResolutionZone;
    [SerializeField] private CardPlayZone selectedCardZone;
    [SerializeField] private CardPlayZone destinationCardZone;
    [SerializeField] private CardPlayZone sourceCardZone;
    [SerializeField] private GameObject uiView;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private BattleState battleState;
    [SerializeField] private BattlePlayerTargetingState targetingState;

    [ReadOnly, SerializeField] private Card _card;
    private Member _hero;
    private int _actionIndex;
    private int _numActions;
    private Target[] _actionTargets;
    
    protected void OnEnable() => selectedCardZone.OnZoneCardsChanged.Subscribe(BeginSelection, this);
    protected void OnDisable() => selectedCardZone.OnZoneCardsChanged.Unsubscribe(this);

    private void BeginSelection()
    {
        if (selectedCardZone.Count < 1)
            return;

        battleState.SelectionStarted = true;
        _card = selectedCardZone.Cards[0];
        Message.Publish(new TargetSelectionBegun(_card));

        var cardClass = _card.LimitedToClass;
        if (!cardClass.IsPresent)
        {
            Debug.Log($"Card {_card.Name} is not playable by Heroes", _card);
            return;
        }

        cardPresenter.Set(_card, () => { });
        Debug.Log($"Showing Selected Card {_card.Name}", gameObject);
        uiView.SetActive(true);

        _hero = battleState.Members.Values.FirstOrDefault(x => x.Class.Equals(cardClass.Value.Name));
        if (_hero == null)
        {
            Debug.Log($"Could not find Party Member with Class {cardClass.Value}");
            return;
        }

        _actionIndex = 0;
        _numActions = _card.ActionSequences.Length;
        _actionTargets = new Target[_numActions];
        if (_numActions == 0)
        {
            Debug.Log($"Card {_card.Name} has no Card Actions");
            OnTargetConfirmed();
            return;
        }

        PresentPossibleTargets();
    }

    private void PresentPossibleTargets()
    {
        var action = _card.ActionSequences[_actionIndex];
        var possibleTargets = battleState.GetPossibleConsciousTargets(_hero, action.Group, action.Scope);
        targetingState.WithPossibleTargets(possibleTargets);
        if (possibleTargets.Length == 1)
            OnTargetConfirmed();
    }

    public void Cancel() => OnCancelled();
    public void OnCancelled()
    {
        OnSelectionComplete(sourceCardZone);
    }

    public void Confirm() => OnTargetConfirmed();
    public void OnTargetConfirmed()
    {
        _actionTargets[_actionIndex] = targetingState.Current;
        targetingState.Clear();

        if (_actionIndex + 1 == _numActions)
        {
            var hero = battleState.Heroes.First(x => x.Id == _hero.Id);
            var playedCard = new PlayedCardV2(hero, _actionTargets, _card);
            cardResolutionZone.Add(playedCard);
            OnSelectionComplete(destinationCardZone);
        }
        else
        {
            _actionIndex++;
            PresentPossibleTargets();
        }
    }

    private void OnSelectionComplete(CardPlayZone sendToZone)
    {
        sendToZone.PutOnBottom(selectedCardZone.DrawOneCard());
        _card = null;
        uiView.SetActive(false);
        battleState.SelectionStarted = false;
        Message.Publish(new TargetSelectionFinished());
    }
}
