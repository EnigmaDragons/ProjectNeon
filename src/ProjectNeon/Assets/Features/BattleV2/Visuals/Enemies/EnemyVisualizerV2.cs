using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class EnemyVisualizerV2 : OnMessage<MemberRevived, CharacterAnimationRequested, ShowHeroBattleThought, SetEnemiesUiVisibility>
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
    
    private readonly Dictionary<EnemyInstance, I2ProgressiveTextRevealWorld> _speech = new Dictionary<EnemyInstance, I2ProgressiveTextRevealWorld>();
    private List<Tuple<int, Member>> _enemyPositions;
    private bool _enemyVisualsVisible = true;
    private bool _techPointsVisible = true;
    private bool _enemyStatsVisible = true;
    
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

    public void RandomizeEnemyPositions()
    {
        var shuffledEnemies = _enemyPositions.Where(x => x.Item2.IsConscious() && state.GetMaybeTransform(x.Item2.Id).IsPresent).ToArray().Shuffled();
        for (var i = 0; i < shuffledEnemies.Length; i++)
        {
            var x = i * widthBetweenEnemies;
            var y = rowUsesYAxis ? (i % 2) * rowHeight : 0;
            var z = !rowUsesYAxis ? (i % 2) * rowHeight : 0;
            state.GetMaybeTransform(shuffledEnemies[i].Item2.Id).Value.localPosition = transform.localPosition - new Vector3(x, y, z);
        }
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
        try
        {
            var enemyObject = Instantiate(enemy.Prefab, transform);
            active.Add(enemyObject);
            enemyArea.WithUiTransforms(active.Select(a => a.transform));
            var speech = enemyObject.GetComponentInChildren<I2ProgressiveTextRevealWorld>();
            if (speech != null)
                _speech[enemy] = speech;
            return enemyObject;
        }
        catch(Exception e)
        {
            Log.Error($"Cannot Instantiate {enemy.NameTerm.ToEnglish()}");
            Log.Error(e);
            return null;
        }
    }

    private GameObject AddEnemy(EnemyInstance enemy, Member member, Vector3 offset, Maybe<Member> isReplacing)
    {
        var enemyObject = Instantiate(enemy.Prefab, transform);
        active.Add(enemyObject);
        var t = enemyObject.transform;
        var iForIndex = _enemyPositions.Where(x => x.Item2.IsConscious()).Max(x => x.Item1) + 1;
        var iForPositioning = isReplacing.IsPresent
            ? _enemyPositions.First(x => x.Item2.Id == isReplacing.Value.Id).Item1
            : iForIndex;
        _enemyPositions.Add(new Tuple<int, Member>(iForIndex, member));
        t.localPosition = transform.localPosition - new Vector3(
            iForPositioning * widthBetweenEnemies, 
            rowUsesYAxis ? (iForPositioning % 2) * rowHeight : 0, 
            !rowUsesYAxis ? (iForPositioning % 2) * rowHeight : 0) + offset;
        return enemyObject;
    }
    
    public void AfterBattleStateInitialized()
    {
        for (var i = 0; i < enemyArea.Enemies.Count; i++)
        {
            var member = state.GetMemberByEnemyIndex(i);
            SetupEnemyUi(enemyArea.Enemies[i], member, enemyArea.EnemyUiPositions[i].gameObject.transform);
            SetupVisualComponents(active[i], member);
        }
    }

    private void SetupVisualComponents(GameObject obj, Member member)
    {
        obj.GetCharacterMouseHover(member.NameTerm).Init(member);

        var stealth = obj.GetComponentInChildren<StealthTransparency>();
        if (stealth == null)
            Log.Info($"{member.NameTerm.ToEnglish()} is missing a {nameof(StealthTransparency)}");
        else
            stealth.Init(member);

        var stealth2 = obj.GetComponentInChildren<CharacterCreatorStealthTransparency>();
        if (stealth2 == null)
            Log.Info($"{member.NameTerm.ToEnglish()} is missing a {nameof(CharacterCreatorStealthTransparency)}");
        else
            stealth2.Init(member);

        var shield = obj.GetComponentInChildren<ShieldVisual>();
        if (shield == null)
            Log.Info($"{member.NameTerm.ToEnglish()} is missing a {nameof(ShieldVisual)}");
        else
            shield.Init(member);
        
        InitRequired<TauntEffect>(member, obj);
        InitRequired<StunnedDisabledEffect>(member, obj);
        InitRequired<BlindedEffect>(member, obj);
        InitRequired<MemberHighlighter>(member, obj);
    }
    
    private void InitRequired<TComponent>(Member m, GameObject character) where TComponent: IMemberUi
    {
        var e = character.GetComponentInChildren<TComponent>();
        if (e == null)
            Debug.LogError($"{m.NameTerm.ToEnglish()} is missing a {typeof(TComponent).FullName}");
        else
            e.Init(m);
    }
    
    public EnemySpawnDetails Spawn(EnemyInstance enemy, Vector3 offset, Maybe<Member> isReplacing)
    {
        DevLog.Write($"Spawning {enemy.NameTerm.ToEnglish()}");
        var member = enemy.AsMember(state.GetNextEnemyId());
        var enemyObject = AddEnemy(enemy, member, offset, isReplacing);
        state.AddEnemy(enemy, enemyObject, member);
        SetupEnemyUi(enemy, member, enemyObject.transform);
        SetupVisualComponents(enemyObject, member);
        return new EnemySpawnDetails(enemy, member, enemyObject.transform);
    }
    
    public void Despawn(MemberState enemy)
    {
        DevLog.Write($"Despawning {enemy.NameTerm.ToEnglish()}");
        enemy.HasLeft = true;
        var index = state.GetEnemyIndexByMemberId(enemy.MemberId);
        state.RemoveEnemy(enemy);
        Destroy(active[index]);
        Destroy(uis[index]);
    }

    private void SetupEnemyUi(EnemyInstance enemy, Member member, Transform obj)
    {
        var pos = obj.transform.position;
        var customUi = obj.GetComponentInChildren<EnemyBattleUIPresenter>();
        customUi = customUi != null
            ? customUi.Initialized(enemy, member)
            : Instantiate(ui, pos, Quaternion.identity, obj).Initialized(enemy, member);
        uis.Add(customUi);
        customUi.gameObject.SetActive(_enemyVisualsVisible);
        customUi.SetTechPointVisibility(_techPointsVisible);  
        customUi.SetStatVisibility(_enemyStatsVisible);
    }

    protected override void Execute(MemberRevived m)
    {
        if (!m.Member.TeamType.Equals(TeamType.Enemies)) return;

        state.GetMaybeTransform(m.Member.Id).IfPresent(t => t.gameObject.SetActive(true));
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
            this.SafeCoroutineOrNothing(animator.PlayAnimationUntilFinished(e.Animation.AnimationName, elapsed =>
            {
                DevLog.Write($"Finished {e.Animation} in {elapsed} seconds.");
                Message.Publish(new Finished<CharacterAnimationRequested>());
            }));
    }
    
    protected override void Execute(ShowHeroBattleThought e)
    {
        if (!state.IsEnemy(e.MemberId)) return;
        
        var enemy = state.GetEnemyById(e.MemberId);
        var s = _speech[enemy];
        s.Display(e.Thought, true, false, () => this.SafeCoroutineOrNothing(ExecuteAfterDelayRealtime(() =>
        {
            if (s != null) 
                s.Hide();
        }, 6f)));
        s.Proceed(true);
    }

    protected override void Execute(SetEnemiesUiVisibility msg)
    {
        if (msg.Component == BattleUiElement.EnemyInfo)
        {
            _enemyVisualsVisible = msg.ShouldShow;
            uis.ForEach(u =>
            {
                if (u != null && u.gameObject != null) 
                    u.gameObject.SetActive(msg.ShouldShow);
            });
        }
        else if (msg.Component == BattleUiElement.EnemyTechPoints)
        {
            _techPointsVisible = msg.ShouldShow;
            uis.ForEach(u => u.SetTechPointVisibility(msg.ShouldShow));   
        }
        else if (msg.Component == BattleUiElement.PrimaryStat)
        {
            _enemyStatsVisible = msg.ShouldShow;
            uis.ForEach(u => u.SetStatVisibility(msg.ShouldShow));   
        }
    }

    private IEnumerator ExecuteAfterDelayRealtime(Action a, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        a();
    }
}
