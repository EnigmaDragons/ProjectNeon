using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;

public class EnemyDetailsView : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Enemy staringEnemy;
    [SerializeField] private TextMeshProUGUI idLabel;
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private Localize typeLabel;
    [SerializeField] private Localize descriptionLocalize;
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
    private const string Elite = "Elite";
    private const string Hasty = "Hasty ";
    private const string Ultimate = "Ultimate";
    private const string Reaction = "Reaction";
    
    private void Awake()
    {
        if (!_isInitialized && staringEnemy != null)
            Show(staringEnemy.ForStage(currentAdventureProgress.AdventureProgress.CurrentChapterNumber), Maybe<Member>.Missing());
    }

    public void Show(EnemyInstance e, Maybe<Member> possibleMember)
    {
        _isInitialized = true;
        idLabel.text = $"#{e.EnemyId.ToString().PadLeft(3, '0')}";
        nameLocalize.SetTerm(e.NameTerm);
        var eliteText = e.Tier == EnemyTier.Elite ? Elite : string.Empty;
        var hastyText = e.IsHasty ? Hasty : string.Empty;
        var typeTerm = (hastyText + eliteText).Trim();
        if (typeLabel != null)
        {
            if (string.IsNullOrWhiteSpace(typeTerm))
                typeLabel.SetFinalText("");
            else
                typeLabel.SetTerm($"BattleUI/{typeTerm}");
        }

        if (statPanel != null)
            statPanel.Initialized(e.Stats);
        
        var member = possibleMember.OrDefault(() => e.AsMember(InfoMemberId.Get()));
        if (specialPowers != null && icons != null)
        {
            var powers = e.StartOfBattleEffects
                    .Where(x => x.StatusTag != StatusTag.None)
                    .Select(s => new CurrentStatusValue
                    {
                        Type = s.StatusTag.ToString(),
                        Icon = icons[s.StatusTag].Icon,
                        Tooltip = s.StatusDetailText
                    })
                .Concat(e.StartOfBattleEffects
                    .Where(x => x.EffectType == EffectType.EnterStealth)
                    .Take(1)
                    .Select(s => new CurrentStatusValue 
                    { 
                        Type = TemporalStatType.Stealth.ToString(), 
                        Icon = icons[TemporalStatType.Stealth].Icon,
                        Tooltip = "Start Battle Stealthed"
                    })).ToList();
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
                .Select(x => (x, x.Tags.Contains(CardTag.Ultimate) ? Ultimate : string.Empty))
                    .Concat(e.ReactionCards.Select(x => ((CardTypeData)x, Reaction)))
                .Select(c => (c.Item1.CreateInstance(-1, member), c.Item2)));
        if (corpUi != null)
            corpUi.Init(e.Corp);
        if (resources != null)
            resources.Init(e);
        if (hasUnshownCardsItem != null && cardsView != null)
            hasUnshownCardsItem.SetActive(e.Cards.Distinct().Count() > cardsView.MaxCardsDisplayed);
        if (descriptionLocalize != null)
            descriptionLocalize.SetTerm(e.DescriptionTerm);
        otherViews.ForEach(o => o.Init(member));
        Message.Publish(new ShowEnemyOnStage(e));
    }

    public string[] GetLocalizeTerms() => new[] { "BattleUI/Hasty", "BattleUI/Elite", "BattleUI/Hasty Elite" };
}
