using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EnemyVisualizerV2 : OnMessage<MemberUnconscious, MemberRevived, CharacterAnimationRequested>
{
    [SerializeField] private BattleState state;
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EnemyBattleUIPresenter ui;
    [SerializeField] private float rowHeight = 1.5f;
    [SerializeField] private float widthBetweenEnemies = 1.5f;

    [ReadOnly, SerializeField] private List<GameObject> active = new List<GameObject>();
    [ReadOnly, SerializeField] private List<EnemyBattleUIPresenter> uis = new List<EnemyBattleUIPresenter>();
    
    public IEnumerator Setup()
    {
        active.ForEach(Destroy);
        active = new List<GameObject>();
        uis = new List<EnemyBattleUIPresenter>();
        var enemies = enemyArea.Enemies;
        for (var i = 0; i < enemies.Count; i++) 
            InstantiateEnemyVisuals(enemies[i].Prefab, i);

        yield break;
    }

    private Transform InstantiateEnemyVisuals(GameObject prefab, int i)
    {
        var enemyObject = Instantiate(prefab, transform);
        active.Add(enemyObject);
        var t = enemyObject.transform;
        t.position = transform.position + new Vector3(i * widthBetweenEnemies, (i % 2) * rowHeight, (i % 2) == 0 ? 0 : 1);
        enemyArea.WithUiTransforms(active.Select(a => a.transform));
        return t;
    }
    
    public void AfterBattleStateInitialized()
    {
        var enemies = enemyArea.Enemies;
        var positions = enemyArea.EnemyUiPositions;
        for (var i = 0; i < enemies.Count; i++)
            InstantiateEnemyUi(i, positions[i].gameObject.transform);
    }

    public void Spawn(Enemy enemy)
    {
        BattleLog.Write($"Spawning {enemy.Name}");
        var index = active.Count;
        var enemyObject = InstantiateEnemyVisuals(enemy.Prefab, index);
        state.AddEnemy(enemy, enemyObject);
        InstantiateEnemyUi(index, enemyObject);
    }

    private void InstantiateEnemyUi(int index, Transform enemyObject)
    {
        var enemyMember = state.GetMemberByEnemyIndex(index);
        var pos = enemyObject.transform.position;
        uis.Add(Instantiate(ui, pos, Quaternion.identity, enemyObject)
            .Initialized(enemyMember));
    }

    private IEnumerator ExecuteAfterDelay(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a();
    }

    protected override void Execute(MemberUnconscious m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Enemies)) return;

        var enemy = state.GetEnemyById(m.Member.Id);
        if (!string.IsNullOrWhiteSpace(enemy.DeathEffect))
        {
            Message.Publish(new BattleEffectAnimationRequested
            {
                EffectName = enemy.DeathEffect, 
                Scope = Scope.One, 
                Target = new Single(m.Member), 
                Group = Group.Self,
                PerformerId = m.Member.Id
            });
        }

        var t = state.GetTransform(m.Member.Id);
        t.DOPunchScale(new Vector3(8, 8, 8), 2, 1);
        t.DOSpiral(2);
        StartCoroutine(ExecuteAfterDelay(() => t.gameObject.SetActive(false), 2));
    }

    protected override void Execute(MemberRevived m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Enemies)) return;

        state.GetTransform(m.Member.Id).gameObject.SetActive(true);
        uis.Where(u => u.Contains(m.Member)).ForEach(u => u.gameObject.SetActive(true));
    }

    protected override void Execute(CharacterAnimationRequested e)
    {
        if (!state.IsEnemy(e.MemberId)) return;

        var enemyIndex = state.GetEnemyIndexByMemberId(e.MemberId);
        var enemy = active[enemyIndex];
        Log.Info($"Began Animation for {enemy.name}");
        var animator = enemy.GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogWarning($"No Animator found for {enemy.name}");
        else
            StartCoroutine(animator.PlayAnimationUntilFinished(e.Animation.AnimationName, elapsed =>
            {
                Log.Info($"Finished {e.Animation} in {elapsed} seconds.");
                Message.Publish(new Finished<CharacterAnimationRequested>());
            }));
    }
}
