using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyVisualizerV2 : OnMessage<MemberUnconscious, CharacterAnimationRequested>
{
    [SerializeField] private BattleState state;
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EnemyBattleUIPresenter ui;
    [SerializeField] private float rowHeight = 1.5f;
    [SerializeField] private float widthBetweenEnemies = 1.5f;

    [ReadOnly, SerializeField] private GameObject[] active = new GameObject[0];
    
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
            var enemyMember = state.GetMemberByEnemyIndex(i);
            var pos = enemyObject.transform.position;
            Instantiate(ui, pos, Quaternion.identity, transform)
                .Initialized(enemyObject,pos, enemyMember);
        }
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
            StartCoroutine(animator.PlayAnimationUntilFinished(e.Animation, elapsed =>
            {
                Log.Info($"Finished {e.Animation} in {elapsed} seconds.");
                Message.Publish(new Finished<CharacterAnimationRequested>());
            }));
    }
}
