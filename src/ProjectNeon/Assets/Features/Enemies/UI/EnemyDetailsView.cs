using System.Collections.Generic;
using System.Linq;
using Features.GameProgression;
using TMPro;
using UnityEngine;

public class EnemyDetailsView : MonoBehaviour
{
    [SerializeField] private Enemy staringEnemy;
    [SerializeField] private TextMeshProUGUI idLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI typeLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;
    [SerializeField] private MemberStatPanel statPanel;
    [SerializeField] private MemberUiBase[] otherViews;
    [SerializeField] private ReadOnlyEnemyDeckUI enemyDeckUi;
    [SerializeField] private SimpleCardsView cardsView;
    [SerializeField] private GameObject hasUnshownCardsItem;
    [SerializeField] private CorpUiBase corpUi;
    [SerializeField] private EnemyResourceInfoPresenter resources;
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;
    [SerializeField] private MemberSpecialPowersUi specialPowers;
    [SerializeField] private StatusIcons icons;

    private bool _isInitialized;
    
    private void Awake()
    {
        if (!_isInitialized && staringEnemy != null)
            Show(staringEnemy.ForStage(currentAdventureProgress.AdventureProgress.CurrentChapterNumber));
    }

    public void Show(EnemyInstance e)
    {
        _isInitialized = true;
        idLabel.text = $"#{e.EnemyId.ToString().PadLeft(3, '0')}";
        nameLabel.text = e.Name;
        var eliteText = e.Tier == EnemyTier.Elite ? "Elite" : "";
        if (typeLabel != null)
            typeLabel.text = eliteText;
        if (statPanel != null)
            statPanel.Initialized(e.Stats);
        
        var member = e.AsMember(InfoMemberId.Get());
        if (specialPowers != null && icons != null)
        {
            var powers = e.StartOfBattleEffects.Where(x => x.StatusTag != StatusTag.None).Select(s =>
                new CurrentStatusValue
                {
                    Type = s.StatusTag.ToString(),
                    Icon = icons[s.StatusTag].Icon,
                    Tooltip = s.StatusDetailText
                }).ToList();
            specialPowers.UpdateStatuses(powers);
        }

        var enemyCards = e.Cards.ToArray();
        if (enemyDeckUi != null)
            enemyDeckUi.Show(enemyCards, member);
        if (cardsView != null)
            cardsView.Show(enemyCards.Cast<CardTypeData>().Concat(e.ReactionCards).Select(c => c.CreateInstance(-1, member)));
        if (corpUi != null)
            corpUi.Init(e.Corp);
        if (resources != null)
            resources.Init(e);
        if (hasUnshownCardsItem != null && cardsView != null)
            hasUnshownCardsItem.SetActive(e.Cards.Distinct().Count() > cardsView.MaxCardsDisplayed);
        if (descriptionLabel != null)
            descriptionLabel.text = e.Description;
        otherViews.ForEach(o => o.Init(member));
        Message.Publish(new ShowEnemyOnStage(e));
    }
}
