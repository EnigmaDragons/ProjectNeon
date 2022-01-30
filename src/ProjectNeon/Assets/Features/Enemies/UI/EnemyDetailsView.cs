using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
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
        var hastyText = e.IsHasty ? "Hasty " : "";
        if (typeLabel != null)
            typeLabel.text = hastyText + eliteText;
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
            e.StartOfBattleEffects.Where(x => x.EffectType == EffectType.EnterStealth)
                .ForEach(s => powers.Add(new CurrentStatusValue 
                { 
                    Type = TemporalStatType.Stealth.ToString(), 
                    Icon = icons[TemporalStatType.Stealth].Icon,
                    Tooltip = "Start Battle Stealthed"
                }));
            specialPowers.UpdateStatuses(powers);
        }

        var enemyCards = e.Cards.ToArray();
        if (enemyDeckUi != null)
            enemyDeckUi.Show(enemyCards, member);
        if (cardsView != null)
            cardsView.Show(enemyCards
                .Cast<CardTypeData>()
                .OrderBy(x => x.Tags.Contains(CardTag.Ultimate) ? 1 : 0)
                .ThenBy(x => x.Tags.Contains(CardTag.Focus) ? 0 : 1)
                .ThenBy(x => x.Cost.BaseAmount)
                .Select(x => ((CardTypeData)x, (x.Tags.Contains(CardTag.Ultimate) ? "Ultimate" : "")))
                .Concat(e.ReactionCards.Select(x => ((CardTypeData)x, "Reaction")))
                .Select(c => (c.Item1.CreateInstance(-1, member), c.Item2)));
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
