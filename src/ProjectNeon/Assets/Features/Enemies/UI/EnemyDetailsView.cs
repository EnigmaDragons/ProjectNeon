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
    [SerializeField] private MemberStatPanel statPanel;
    [SerializeField] private MemberUiBase[] otherViews;
    [SerializeField] private ReadOnlyEnemyDeckUI enemyDeckUi;
    [SerializeField] private SimpleCardsView cardsView;
    [SerializeField] private GameObject hasUnshownCardsItem;
    [SerializeField] private CorpUiBase corpUi;
    [SerializeField] private EnemyResourceInfoPresenter resources;
    [SerializeField] private CurrentAdventureProgress currentAdventureProgress;

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
        if (typeLabel != null)
            typeLabel.text = e.Role.ToString();
        statPanel.Initialized(e.Stats);
        var member = e.AsMember(InfoMemberId.Get());
        enemyDeckUi.Show(e.Cards, member);
        if (cardsView != null)
            cardsView.Show(e.Cards.Select(c => c.CreateInstance(-1, member)));
        if (corpUi != null)
            corpUi.Init(e.Corp);
        if (resources != null)
            resources.Init(e);
        if (hasUnshownCardsItem != null && cardsView != null)
            hasUnshownCardsItem.SetActive(e.Cards.Distinct().Count() > cardsView.MaxCardsDisplayed);
        otherViews.ForEach(o => o.Init(member));
        Message.Publish(new ShowEnemyOnStage(e));
    }
}
