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
    [SerializeField] private float rowHeight = 16f;
    [SerializeField] private float widthBetweenEnemies = 8f;
    [SerializeField] private bool rowUsesYAxis = true;
    [SerializeField] private CurrentAnimationContext animationContext;

    [ReadOnly, SerializeField] private List<GameObject> active = new List<GameObject>();
    [ReadOnly, SerializeField] private List<EnemyBattleUIPresenter> uis = new List<EnemyBattleUIPresenter>();

    private List<Tuple<int, Member>> _enemyPositions; 
    
    public IEnumerator Spawn()
    {
        active.ForEach(Destroy);
        active = new List<GameObject>();
        uis = new List<EnemyBattleUIPresenter>();
        var enemies = enemyArea.Enemies;
        for (var i = 0; i < enemies.Count; i++) 
            InstantiateEnemyVisuals(enemies[i]);

        yield break;
    }

    public void Place(List<Tuple<int, Member>> enemyPositions)
    {
        _enemyPositions = enemyPositions;
        foreach (var enemyPosition in enemyPositions)
        {
            var enemyObject = active[enemyPosition.Item1].transform;
            var t = enemyObject.transform;
            var x = enemyPosition.Item1 * widthBetweenEnemies;
            var y = rowUsesYAxis ? (enemyPosition.Item1 % 2) * rowHeight : 0;
            var z = !rowUsesYAxis ? (enemyPosition.Item1 % 2) * rowHeight : 0;
            t.localPosition = transform.localPosition - new Vector3(x, y, z);
        }
    }

    private GameObject InstantiateEnemyVisuals(EnemyInstance enemy)
    {
        var enemyObject = Instantiate(enemy.Prefab, transform);
        active.Add(enemyObject);
        enemyArea.WithUiTransforms(active.Select(a => a.transform));
        return enemyObject;
    }

    private GameObject AddEnemy(EnemyInstance enemy, Member member, Vector3 offset)
    {
        var enemyObject = Instantiate(enemy.Prefab, transform);
        active.Add(enemyObject);
        var t = enemyObject.transform;
        var i = _enemyPositions.Max(x => x.Item1) + 1;
        var replacement = _enemyPositions.OrderBy(x => x.Item1).FirstOrDefault(x => !x.Item2.IsConscious());
        if (replacement != null)
        {
            i = replacement.Item1;
            _enemyPositions.Remove(replacement);
            _enemyPositions.Add(new Tuple<int, Member>(i, member));
        }
        else
        {
            _enemyPositions.Add(new Tuple<int, Member>(i, member));
        }
        t.localPosition = transform.localPosition - new Vector3(i * widthBetweenEnemies, (i % 2) * rowHeight, (i % 2) == 0 ? 0 : 1) + offset;
        return enemyObject;
    }
    
    public void AfterBattleStateInitialized()
    {
        for (var i = 0; i < enemyArea.Enemies.Count; i++)
        {
            var member = state.GetMemberByEnemyIndex(i);
            SetupEnemyUi(member, enemyArea.EnemyUiPositions[i].gameObject.transform);
            SetupVisualComponents(active[i], member);
        }
    }

    private void SetupVisualComponents(GameObject obj, Member member)
    {
        obj.GetCharacterMouseHover(member.Name).Init(member);

        var stealth = obj.GetComponentInChildren<StealthTransparency>();
        if (stealth == null)
            Log.Info($"{member.Name} is missing a {nameof(StealthTransparency)}");
        else
            stealth.Init(member);

        var shield = obj.GetComponentInChildren<ShieldVisual>();
        if (shield == null)
            Log.Info($"{member.Name} is missing a {nameof(ShieldVisual)}");
        else
            shield.Init(member);
        
        var highlighter = obj.GetComponentInChildren<MemberHighlighter>();
        if (highlighter == null)
            Debug.LogError($"{member.Name} is missing a {nameof(MemberHighlighter)}");
        else
            highlighter.Init(member);
    }
    
    public Member Spawn(EnemyInstance enemy, Vector3 offset)
    {
        DevLog.Write($"Spawning {enemy.Name}");
        var member = enemy.AsMember(state.GetNextEnemyId());
        var enemyObject = AddEnemy(enemy, member, offset);
        state.AddEnemy(enemy, enemyObject, member);
        SetupEnemyUi(member, enemyObject.transform);
        return member;
    }
    
    public void Despawn(MemberState enemy)
    {
        DevLog.Write($"Despawning {enemy.Name}");
        var index = state.GetEnemyIndexByMemberId(enemy.MemberId);
        state.RemoveEnemy(enemy);
        Destroy(active[index]);
        Destroy(uis[index]);
    }

    private void SetupEnemyUi(Member member, Transform obj)
    {
        var pos = obj.transform.position;
        var customUi = obj.GetComponentInChildren<EnemyBattleUIPresenter>();
        uis.Add(customUi != null
            ? customUi.Initialized(member)
            : Instantiate(ui, pos, Quaternion.identity, obj).Initialized(member));
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
        this.ExecuteAfterDelay(() => t.gameObject.SetActive(false), 2.2f);
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
        DevLog.Write($"Playing Enemy Animation {e.Animation.AnimationName}");
        
        animationContext.SetAnimation(e);
            
        var enemyIndex = state.GetEnemyIndexByMemberId(e.MemberId);
        var enemy = active[enemyIndex];
        var animator = enemy.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            DevLog.Write($"No Animator found for {enemy.name}");
            Message.Publish(new Finished<CharacterAnimationRequested>());
        }
        else
            StartCoroutine(animator.PlayAnimationUntilFinished(e.Animation.AnimationName, elapsed =>
            {
                DevLog.Write($"Finished {e.Animation} in {elapsed} seconds.");
                Message.Publish(new Finished<CharacterAnimationRequested>());
            }));
    }
}
