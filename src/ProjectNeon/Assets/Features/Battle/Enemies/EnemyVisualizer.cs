using UnityEngine;

/**
 * Puts the enemy party into screen.
 * 
 * Display enemies into screen in the following format (each number is enemies variable index):
 * 0 2 4 6
 *  1 3 5
 * Uses enemy sprites with default size of 100 x 200
 */
public class EnemyVisualizer : MonoBehaviour
{
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private GameObject enemyPrototype;
    [SerializeField] private GameEvent onSetupFinished;
    [SerializeField] private float rowHeight = 1.5f;
    [SerializeField] private float widthBetweenEnemies = 1.5f; 

    public void SetupEnemies()
    {
        for (var i= 0; i < enemyArea.enemies.Count; i++)
        {
            var enemy = Instantiate(enemyPrototype, transform);
            var r = enemy.AddComponent<SpriteRenderer>();
            r.sprite = enemyArea.enemies[i].image;
            r.transform.position = transform.position + new Vector3(i * widthBetweenEnemies, (i % 2) * rowHeight, 0);
        }
        onSetupFinished.Publish();
    }
}
