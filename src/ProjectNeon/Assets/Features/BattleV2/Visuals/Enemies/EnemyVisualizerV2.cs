using System.Collections;
using UnityEngine;

public class EnemyVisualizerV2 : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private WorldHPBarController hpBarPrototype;
    [SerializeField] private Vector3 hpBarOffset;
    [SerializeField] private DamageEffect damageEffect;
    [SerializeField] private Vector3 damageEffectOffset;
    [SerializeField] private float rowHeight = 1.5f;
    [SerializeField] private float widthBetweenEnemies = 1.5f;

    [ReadOnly, SerializeField] private GameObject[] active = new GameObject[0];
    
    private void OnEnable() => BattleEvent.Subscribe<MemberUnconscious>(ResolveUnconscious, this);
    private void OnDisable() => BattleEvent.Unsubscribe(this);

    public IEnumerator Setup()
    {
        active.ForEach(Destroy);
        active = new GameObject[enemyArea.Enemies.Length];
        var enemies = enemyArea.Enemies;
        var positions = new Transform[enemies.Length];
        for (var i = 0; i < enemies.Length; i++)
        {
            var enemyObject = Instantiate(enemies[i].Prefab, transform);
            active[i] = enemyObject;
            var t = enemyObject.transform;
            t.position = transform.position + new Vector3(i * widthBetweenEnemies, (i % 2) * rowHeight, 0);
            positions[i] = t;
        }

        enemyArea.WithUiTransforms(positions);
        yield break;
    }

    public void AfterBattleStateInitialized()
    {
        var enemies = enemyArea.Enemies;
        var positions = enemyArea.EnemyUiPositions;
        for (var i = 0; i < enemies.Length; i++)
        {
            var enemyObject = positions[i].gameObject;
            var hpBar = Instantiate(hpBarPrototype, enemyObject.transform.position + hpBarOffset, Quaternion.identity, enemyObject.transform);
            var enemyMember = state.GetMemberByEnemyIndex(i);
            hpBar.Init(enemyMember);
            var dmg = Instantiate(damageEffect, enemyObject.transform.position + damageEffectOffset, Quaternion.identity, enemyObject.transform);
            dmg.Init(enemyMember);
        }
    }
    
    private void ResolveUnconscious(MemberUnconscious m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Enemies)) return;

        var enemy = state.GetEnemyById(m.Member.Id);
        if (!string.IsNullOrWhiteSpace(enemy.DeathEffect))
        {
            BattleEvent.Publish(new BattleEffectAnimationRequested
            {
                EffectName = enemy.DeathEffect, 
                Scope = Scope.One, 
                Target = new Single(m.Member), 
                Group = Group.Self,
                PerformerId = m.Member.Id
            });
        }

        var t = state.GetTransform(m.Member.Id);
        t.gameObject.SetActive(false);
    }
}
