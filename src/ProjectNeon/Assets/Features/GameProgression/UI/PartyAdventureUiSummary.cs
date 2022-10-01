using System.Collections.Generic;
using UnityEngine;

public class PartyAdventureUiSummary : OnMessage<PartyAdventureStateChanged, PartyStateChanged>
{
    [SerializeField] private CurrentAdventure current;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureHeroUiSummary heroPresenter;
    [SerializeField] private GameObject itemParent;
    [SerializeField] private RectTransform backPanel;
    [SerializeField] private Vector2 backPanelOffsetIfLessThan3Heroes;

    private bool _offsetApplied;
    
    [ReadOnly, SerializeField] private List<AdventureHeroUiSummary> active = new List<AdventureHeroUiSummary>();

    private void Awake() => UpdateUiSummary();
    protected override void Execute(PartyAdventureStateChanged msg) => UpdateUiSummary();
    protected override void Execute(PartyStateChanged msg) => UpdateUiSummary();

    private void UpdateUiSummary()
    {
        ToggleOffset(party.BaseHeroes.Length);
        active.ForEach(Destroy);
        active.Clear();
        itemParent.DestroyAllChildren();
        
        for (var i = 0; i < party.BaseHeroes.Length; i++)
        {
            var h = Instantiate(heroPresenter, itemParent.transform);
            h.Init(party.Heroes[i], current.Adventure.IsV1);
            active.Add(h);
        }
    }

    private void ToggleOffset(int partySize)
    {
        if (partySize < 3 && _offsetApplied)
            return;
        if (partySize == 3 && !_offsetApplied)
            return;
        if (partySize < 3 && !_offsetApplied)
            backPanel.anchoredPosition += backPanelOffsetIfLessThan3Heroes;
        if (partySize == 3 && _offsetApplied)
            backPanel.anchoredPosition -= backPanelOffsetIfLessThan3Heroes;
        _offsetApplied = partySize < 3;
    }
}
