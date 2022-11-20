using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

public class PartyLevelAndXpRewardsPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private BattleState state;
    [SerializeField] private HeroLevelAndXpPresenter heroPrototype;
    [SerializeField] private GameObject heroParent;
    [SerializeField] private Localize partyXpGainLabel;

    private List<HeroLevelAndXpPresenter> _presenters = new List<HeroLevelAndXpPresenter>();
    private const string GainedXpTerm = "BattleUI/GainedXP";
    
    private void OnEnable()
    {
        heroParent.DestroyAllChildren();
        _presenters = new List<HeroLevelAndXpPresenter>();
        _presenters = state.Party.Heroes.Select(h => Instantiate(heroPrototype, heroParent.transform).Initialized(h)).ToList();
        var totalXp = state.PredictedTotalRewardXp;
        partyXpGainLabel.SetFinalText(string.Format(GainedXpTerm.ToLocalized(), totalXp));
        this.ExecuteAfterDelay(() => ShowXpPreview(totalXp), 2f);
    }

    private void ShowXpPreview(int xp)
    {
        state.Party.Heroes.ForEachIndex((h, i) => _presenters[i].ShowPreview(h.Levels.PreviewChange(xp)));
    }

    public string[] GetLocalizeTerms()
        => new[] {GainedXpTerm};
}
