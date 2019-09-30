using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private EnemyArea enemies;
    
    public Party Party => party;
    public EnemyArea EnemyArea => enemies;
    public IReadOnlyDictionary<int, Member> Members => _membersById;
    private Dictionary<int, Enemy> _enemiesById = new Dictionary<int, Enemy>();
    private Dictionary<int, Character> _heroesById = new Dictionary<int, Character>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();

    public BattleState Initialized(Party party, EnemyArea enemyArea)
    {
        this.party = party;
        this.enemies = enemyArea;
        return Init();
    }

    // @todo #1:10min Initialize this as the last step in Battle Setup
    public BattleState Init()
    {
        var id = 1;      
        _heroesById = new[] {party.characterOne, party.characterTwo, party.characterThree}.ToDictionary(_ => id++, h => h);
        _enemiesById = enemies.Enemies.ToDictionary(e => id++, e => e);
        _membersById = _heroesById.Select(h => new Member(h.Key, h.Value.name, TeamType.Party, h.Value.Stats))
            .Concat(_enemiesById.Select(e => e.Value.AsMember(e.Key)))
            .ToDictionary(x => x.Id, x => x);
        return this;
    }
    
    public Enemy GetEnemyById(int memberId) => _enemiesById[memberId];
}
