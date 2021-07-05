using UnityEngine;
using UnityEngine.UI;

public class EnemyBestiaryView : MonoBehaviour
{
    [SerializeField] private AllEnemies enemies;
    [SerializeField] private EnemyDetailsView enemyView;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private IndexSelector<Enemy> _enemies;
    
    private void Awake()
    {
        _enemies = new IndexSelector<Enemy>(enemies.Enemies);
        previousButton.onClick.AddListener(MovePrevious);
        nextButton.onClick.AddListener(MoveNext);
    }

    private void Render()
    {
        var enemy = _enemies.Current;
        var firstStage = enemy.Stages[0];
        enemyView.Show(enemy.ForStage(firstStage));
    }

    private void MoveNext()
    {
        _enemies.MoveNext();
        Render();
    }

    private void MovePrevious()
    {
        _enemies.MovePrevious();
        Render();
    }
}
