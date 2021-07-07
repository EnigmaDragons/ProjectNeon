using TMPro;
using UnityEngine;

public class EnemyDetailsView : MonoBehaviour
{
    [SerializeField] private Enemy staringEnemy;
    [SerializeField] private TextMeshProUGUI idLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private MemberStatPanel statPanel;
    [SerializeField] private MemberUiBase[] otherViews;
    [SerializeField] private ReadOnlyEnemyDeckUI enemyDeckUi;
    [SerializeField] private AdventureProgress2 currentAdventureProgress;

    private int _memberId = int.MinValue;
    private bool _isInitialized;
    
    private void Awake()
    {
        if (!_isInitialized && staringEnemy != null)
            Show(staringEnemy.ForStage(currentAdventureProgress.CurrentChapterNumber));
    }

    public void Show(EnemyInstance e)
    {
        _isInitialized = true;
        idLabel.text = $"#{e.EnemyId.ToString().PadLeft(3, '0')}";
        nameLabel.text = e.Name;
        statPanel.Initialized(e.Stats);
        var member = e.AsMember(_memberId++);
        enemyDeckUi.Show(e.Cards, member);
        otherViews.ForEach(o => o.Init(member));
        Message.Publish(new ShowEnemyOnStage(e));
    }
}
