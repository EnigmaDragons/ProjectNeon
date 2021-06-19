using TMPro;
using UnityEngine;

public class EnemyDetailsView : MonoBehaviour
{
    [SerializeField] private Enemy staringEnemy;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private MemberStatPanel statPanel;
    [SerializeField] private ReadOnlyEnemyDeckUI enemyDeckUi;
    [SerializeField] private AdventureProgress2 currentAdventureProgress;

    private bool _isInitialized;
    
    private void Awake()
    {
        if (!_isInitialized && staringEnemy != null)
            Show(staringEnemy.GetEnemy(currentAdventureProgress.CurrentChapterNumber));
    }

    public void Show(EnemyInstance e)
    {
        _isInitialized = true;
        nameLabel.text = e.Name;
        statPanel.Initialized(e.Stats);
        enemyDeckUi.Show(e.Cards, e.AsMember(-1));   
    }
}
