using TMPro;
using UnityEngine;

public class EnemyDetailsView : MonoBehaviour
{
    [SerializeField] private Enemy staringEnemy;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private MemberStatPanel statPanel;
    [SerializeField] private ReadOnlyEnemyDeckUI enemyDeckUi;

    private void Awake()
    {
        if (staringEnemy != null)
            Show(staringEnemy);
    }

    public void Show(Enemy e)
    {
        nameLabel.text = e.Name;
        statPanel.Initialized(e.Stats);
        enemyDeckUi.Show(e.Deck);   
    }
}
