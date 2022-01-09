using System.Linq;
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
        _enemies = new IndexSelector<Enemy>(enemies.Enemies.Where(x => !x.ExcludeFromBestiary).OrderBy(x => x.ForStage(1).PowerLevel).ToArray());
        previousButton.onClick.AddListener(MovePrevious);
        nextButton.onClick.AddListener(MoveNext);
        Render();
    }

    private void Render()
    {
        var enemy = _enemies.Current;
        var firstStage = enemy.Stages[0];
        var enemyInstance = enemy.ForStage(firstStage);
        enemyView.Show(enemyInstance);
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
