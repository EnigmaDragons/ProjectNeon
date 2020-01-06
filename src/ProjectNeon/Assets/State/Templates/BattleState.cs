using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private CardPlayZones cardPlayZones;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private EnemyArea enemies;
    [SerializeField] private GameObject nextBattlegroundPrototype;
    [SerializeField, ReadOnly] private Vector3[] uiPositions;
    [SerializeField, ReadOnly] private string[] memberNames;
    public bool SelectionStarted = false;

    public Party Party => partyArea.Party;
    public PartyArea PartyArea => partyArea;
    public EnemyArea EnemyArea => enemies;
    public GameObject Battlefield => nextBattlegroundPrototype;
    public IReadOnlyDictionary<int, Member> Members => _membersById;
    public Member[] Heroes => Members.Values.Where(x => x.TeamType == TeamType.Party).ToArray();
    public Member[] Enemies => Members.Values.Where(x => x.TeamType == TeamType.Enemies).ToArray();
    private Dictionary<int, Enemy> _enemiesById = new Dictionary<int, Enemy>();
    private Dictionary<int, Hero> _heroesById = new Dictionary<int, Hero>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();
    private Dictionary<int, Transform> _uiTransformsById = new Dictionary<int, Transform>();
    public List<Effect> QueuedEffects { get; private set;  }

    public BattleState Initialized(PartyArea partyArea, EnemyArea enemyArea)
    {
        this.QueuedEffects = new List<Effect>();
        this.partyArea = partyArea;
        this.enemies = enemyArea;
        return Init();
    }

    public void SetNextBattleground(GameObject prototype)
    {
        nextBattlegroundPrototype = prototype;
    }

    private int EnemyStartingIndex => 2;
    public BattleState Init()
    {
        var id = 1;      
        var heroes = Party.Heroes;

        memberNames = new string[EnemyStartingIndex + enemies.Enemies.Length + 3];
        _uiTransformsById = new Dictionary<int, Transform>();
        _enemiesById = new Dictionary<int, Enemy>();
        for (var i = 0; i < enemies.Enemies.Length; i++)
        {
            id++;
            _enemiesById[id] = enemies.Enemies[i];
            _uiTransformsById[id] = enemies.EnemyUiPositions[i];
            memberNames[id] = enemies.Enemies[i].name;
        }
        
        _heroesById = new Dictionary<int, Hero>();
        for (var i = 0; i < 3; i++)
        {
            id++;
            _heroesById[id] = heroes[i];
            _uiTransformsById[id] = partyArea.UiPositions[i];
            memberNames[id] = heroes[i].name;
        }
        
        _membersById = _heroesById.Select(h => new Member(h.Key, h.Value.name, h.Value.ClassName.Value, TeamType.Party, h.Value.Stats))
            .Concat(_enemiesById.Select(e => e.Value.AsMember(e.Key)))
            .ToDictionary(x => x.Id, x => x);

        uiPositions = _uiTransformsById.Values.Select(x => x.position).ToArray();
        
        Debug.Log("Finished Battle State Init");
        return this;
    }

    public bool IsHero(int memberId) => _heroesById.ContainsKey(memberId);
    public Hero GetHeroById(int memberId) => _heroesById[memberId];
    public Enemy GetEnemyById(int memberId) => _enemiesById[memberId];
    public Vector3 GetPosition(int memberId) => _uiTransformsById[memberId].position;
    public Member GetMemberByHero(Hero hero) => _membersById[_heroesById.Single(x => x.Value == hero).Key];
    public Member GetMemberByEnemyIndex(int enemyIndex) => _membersById[enemyIndex + EnemyStartingIndex];
}
