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
    [SerializeField] private GameEvent onSetupFinished;
    [SerializeField] private WorldHPBarController hpBarPrototype;
    [SerializeField] private Vector3 hpBarOffset;
    [SerializeField] private float rowHeight = 1.5f;
    [SerializeField] private float widthBetweenEnemies = 1.5f;

    [ReadOnly, SerializeField] private GameObject[] visuals;
    
    private void OnEnable() => BattleEvent.Subscribe<MemberUnconscious>(ResolveUnconscious, this);
    private void OnDisable() => BattleEvent.Unsubscribe(this);

    public void SetupEnemies()
    {
        visuals = new GameObject[enemyArea.Enemies.Length];
        var enemies = enemyArea.Enemies;
        var positions = new Transform[enemies.Length];
        for (var i= 0; i < enemies.Length; i++)
        {
            var enemy = enemies[i];
            
            var enemyObject = Instantiate(enemies[i].Prefab, transform);
            var t = enemyObject.transform;
            t.position = transform.position + new Vector3(i * widthBetweenEnemies, (i % 2) * rowHeight, 0);
            positions[i] = t;
            
            var hpBar = Instantiate(hpBarPrototype, enemyObject.transform.position + hpBarOffset, Quaternion.identity, enemyObject.transform);
            hpBar.Init((int)enemy.Stats.MaxHP());
        }

        enemyArea.WithUiTransforms(positions);
        onSetupFinished.Publish();
    }
    
    private void ResolveUnconscious(MemberUnconscious m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Enemies)) return;
        
        for (var i = 0; i < enemyArea.Enemies.Length; i++)
            if (enemyArea.name.Equals(m.Member.Name))
                visuals[i].gameObject.SetActive(false);
    }
}
