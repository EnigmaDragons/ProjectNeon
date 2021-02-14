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
    //Initial Solution
    [SerializeField] private float rowHeight = 1.5f;
    [SerializeField] private float widthBetweenEnemies = 1.5f;
    //Solution Attempt 1
    [SerializeField] private Vector2 startingPoint;
    [SerializeField] private float widthDifference;
    [SerializeField] private float heightDifference;
    [SerializeField] private float rowXOffset;
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    //Solution Attempt 2
    [SerializeField] private Vector3[] enemy1;
    [SerializeField] private Vector3[] small2;
    [SerializeField] private Vector3[] small1big1;
    [SerializeField] private Vector3[] big2;
    [SerializeField] private Vector3[] small3;
    [SerializeField] private Vector3[] small2big1;
    [SerializeField] private Vector3[] small1big2;
    [SerializeField] private Vector3[] big3;
    [SerializeField] private Vector3[] small4;
    [SerializeField] private Vector3[] small3big1;
    [SerializeField] private Vector3[] small2big2;
    [SerializeField] private Vector3[] small1big3;
    [SerializeField] private Vector3[] big4;
    [SerializeField] private Vector3[] small5;
    [SerializeField] private Vector3[] small4big1;
    [SerializeField] private Vector3[] small3big2;
    [SerializeField] private Vector3[] small6;
    [SerializeField] private Vector3[] small5big1;
    [SerializeField] private Vector3[] small7;
    [SerializeField] private Vector3[] small6big1;
    [SerializeField] private Vector3[] small8;
    [SerializeField] private Vector3[] small9;
    [SerializeField] private CurrentAnimationContext animationContext;

    [ReadOnly, SerializeField] private List<GameObject> active = new List<GameObject>();
    [ReadOnly, SerializeField] private List<EnemyBattleUIPresenter> uis = new List<EnemyBattleUIPresenter>();

    //Solution 1
    private bool _init;
    private int[,] _enemyMap;
    private Vector2[,] _placementMap;
    //Solution 2
    private Dictionary<int, Dictionary<int, Vector3[]>> _positionMap;
    private List<EnemyInitInformation> _enemies;
    
    private void Init()
    {
        if (_init)
            return;
        _init = true;
        //solution 1
        _enemyMap = new int[columns,rows];
        _placementMap = new Vector2[columns,rows];
        for (var column = 0; column < columns; column++)
        {
            for (var row = 0; row < rows; row++)
            {
                _placementMap[column, row] = new Vector2(
                    startingPoint.x + widthDifference * column + rowXOffset * row, 
                    startingPoint.y + heightDifference * row);
            }
        }
        //solution 2
        _positionMap = new Dictionary<int, Dictionary<int, Vector3[]>>
        {
            { 
                0, 
                new Dictionary<int, Vector3[]>
                {
                    { 1, enemy1 },
                    { 2, small2 },
                    { 3, small3 },
                    { 4, small4 },
                    { 5, small5 },
                    { 6, small6 },
                    { 7, small7 },
                    { 8, small8 },
                    { 9, small9 }
                } 
            },
            { 
                1, 
                new Dictionary<int, Vector3[]>()
                {
                    { 0, enemy1 },
                    { 1, small1big1 },
                    { 2, small2big1 },
                    { 3, small3big1 },
                    { 4, small4big1 },
                    { 5, small5big1 },
                    { 6, small6big1 }
                } 
            },
            {
                2, 
                new Dictionary<int, Vector3[]>
                {
                    { 0, big2 },
                    { 1, small1big2 },
                    { 2, small2big2 },
                    { 3, small3big2 }
                }
            },
            {
                3, 
                new Dictionary<int, Vector3[]>
                {
                    { 0, big3 },
                    { 1, small1big3 }
                }
            },
            {
                4, 
                new Dictionary<int, Vector3[]>
                {
                    { 0, big4 }
                }
            }
        };
    }
    
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

    public void Place(List<EnemyInitInformation> enemyPositions)
    {
        Init();
        var bigCount = enemyPositions.Count(x => x.Enemy.IsBig);
        var smallCount = enemyPositions.Count(x => !x.Enemy.IsBig);
        if (!_positionMap.ContainsKey(bigCount) || !_positionMap[bigCount].ContainsKey(smallCount))
            throw new Exception($"Can't place these enemeies: {bigCount} bigs & {smallCount} smalls");

            _enemyPositions = enemyPositions;
        foreach (var enemyPosition in enemyPositions.OrderBy(x => x.Enemy.Width).ThenBy(x => x.Enemy.Height))
        {
            var enemyObject = active[enemyPosition.Item1].transform;
            var t = enemyObject.transform;
            t.position = transform.position + new Vector3(enemyPosition.Item1 * widthBetweenEnemies, (enemyPosition.Item1 % 2) * rowHeight, (enemyPosition.Item1 % 2) == 0 ? 0 : 1);
        }
    }

    private Tuple<int, int> Place(EnemyInitInformation enemy, int[,] subMap)
    {
        
        if (enemy.Enemy.Width == 3 && enemy.Enemy.Height == 3)
        {
            if (_enemyMap[1, 1] > 0)
                Log.Error($"Not enough room to place enemy {enemy.Enemy.Name}");
            foreach (var  in )
            {
                
            }
        }
    }

    private void PositionEnemies()
    {
        
    }

    private GameObject InstantiateEnemyVisuals(Enemy enemy)
    {
        var enemyObject = Instantiate(enemy.Prefab, transform);
        active.Add(enemyObject);
        enemyArea.WithUiTransforms(active.Select(a => a.transform));
        return enemyObject;
    }

    private GameObject AddEnemy(Enemy enemy, Member member)
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
        t.position = transform.position + new Vector3(i * widthBetweenEnemies, (i % 2) * rowHeight, (i % 2) == 0 ? 0 : 1);
        return enemyObject;
    }
    
    public void AfterBattleStateInitialized()
    {
        var enemies = enemyArea.Enemies;
        var positions = enemyArea.EnemyUiPositions;
        for (var i = 0; i < enemies.Count; i++)
            SetupEnemyUi(state.GetMemberByEnemyIndex(i), positions[i].gameObject.transform);

        for (var i = 0; i < enemies.Count; i++)
        {
            var hoverCharacter = active[i].GetComponentInChildren<HoverCharacter>();
            if (hoverCharacter == null)
                Log.Error($"{enemies[i].Name} is missing a {nameof(HoverCharacter)}");
            else
                hoverCharacter.Init(state.GetMemberByEnemyIndex(i));
        }
    }

    public Member Spawn(Enemy enemy, BattleState battleState)
    {
        DevLog.Write($"Spawning {enemy.Name}");
        var member = enemy.AsMember(state.GetNextEnemyId(), battleState);
        var enemyObject = AddEnemy(enemy, member);
        state.AddEnemy(enemy, enemyObject, member);
        SetupEnemyUi(member, enemyObject.transform);
        return member;
    }

    private void SetupEnemyUi(Member enemyMember, Transform enemyObject)
    {
        var pos = enemyObject.transform.position;
        var customUi = enemyObject.GetComponentInChildren<EnemyBattleUIPresenter>();
        enemyObject.GetComponentInChildren<HoverCharacter>().Init(enemyMember);
        uis.Add(customUi != null
            ? customUi.Initialized(enemyMember)
            : Instantiate(ui, pos, Quaternion.identity, enemyObject).Initialized(enemyMember));
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
        StartCoroutine(ExecuteAfterDelay(() => t.gameObject.SetActive(false), 2.2f));
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
        animationContext.SetAnimation(e);
            
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
