using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PartyLevelAndXpRewardsPresenter : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private HeroLevelAndXpPresenter heroPrototype;
    [SerializeField] private GameObject heroParent;
    [SerializeField] private TextMeshProUGUI partyXpGainLabel;

    private List<HeroLevelAndXpPresenter> _presenters = new List<HeroLevelAndXpPresenter>();

    private void OnEnable()
    {
        heroParent.DestroyAllChildren();
        _presenters = new List<HeroLevelAndXpPresenter>();
        _presenters = state.Party.Heroes.Select(h => Instantiate(heroPrototype, heroParent.transform).Initialized(h)).ToList();
        partyXpGainLabel.text = $"Heroes Gained {state.RewardXp} Xp";
        this.ExecuteAfterDelay(() => ShowXpPreview(state.RewardXp), 2f);
    }

    private void ShowXpPreview(int xp)
    {
        state.Party.Heroes.ForEachIndex((h, i) => _presenters[i].ShowPreview(h.Levels.PreviewChange(xp)));
    }
}
