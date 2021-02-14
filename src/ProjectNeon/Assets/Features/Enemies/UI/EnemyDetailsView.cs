using TMPro;
using UnityEngine;

public class EnemyDetailsView : MonoBehaviour
{
    [SerializeField] private Enemy staringEnemy;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private MemberStatPanel statPanel;
    [SerializeField] private ReadOnlyEnemyDeckUI enemyDeckUi;

    private bool _isInitialized;
    
    private void Awake()
    {
        if (!_isInitialized && staringEnemy != null)
            Show(staringEnemy);
    }

    public void Show(Enemy e)
    {
        _isInitialized = true;
        nameLabel.text = e.Name;
        statPanel.Initialized(e.Stats);
        enemyDeckUi.Show(e.Deck);   
    }
}
