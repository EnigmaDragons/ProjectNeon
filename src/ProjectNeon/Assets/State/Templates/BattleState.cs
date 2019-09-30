using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private EnemyArea enemies;
    [SerializeField, ReadOnly] private Vector3[] uiPositions;
    
    public Party Party => partyArea.Party;
    public EnemyArea EnemyArea => enemies;
    public IReadOnlyDictionary<int, Member> Members => _membersById;
    private Dictionary<int, Enemy> _enemiesById = new Dictionary<int, Enemy>();
    private Dictionary<int, Hero> _heroesById = new Dictionary<int, Hero>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();
    private Dictionary<int, Transform> _uiTransformsById = new Dictionary<int, Transform>();

    public BattleState Initialized(PartyArea partyArea, EnemyArea enemyArea)
    {
        this.partyArea = partyArea;
        this.enemies = enemyArea;
        return Init();
    }

    public BattleState Init()
    {
        var id = 1;      
        var heroes = new[] {Party.heroOne, Party.heroTwo, Party.heroThree};
        
        _uiTransformsById = new Dictionary<int, Transform>();
        _enemiesById = new Dictionary<int, Enemy>();
        for (var i = 0; i < enemies.Enemies.Length; i++)
        {
            id++;
            _enemiesById[id] = enemies.Enemies[i];
            _uiTransformsById[id] = enemies.EnemyUiPositions[i];
        }
        
        _heroesById = new Dictionary<int, Hero>();
        for (var i = 0; i < 3; i++)
        {
            id++;
            _heroesById[id] = heroes[i];
            _uiTransformsById[id] = partyArea.UiPositions[i];
        }
        
        _membersById = _heroesById.Select(h => new Member(h.Key, h.Value.name, TeamType.Party, h.Value.Stats))
            .Concat(_enemiesById.Select(e => e.Value.AsMember(e.Key)))
            .ToDictionary(x => x.Id, x => x);

        uiPositions = _uiTransformsById.Values.Select(x => x.position).ToArray();
        return this;
    }
    
    public Enemy GetEnemyById(int memberId) => _enemiesById[memberId];
    public Vector3 GetPosition(int memberId) => _uiTransformsById[memberId].position;
}
