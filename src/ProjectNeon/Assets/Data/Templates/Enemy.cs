using UnityEngine;

public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int powerLevel;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int maxHp;
    [SerializeField] private ResourceType resourceType;

    public Deck Deck => deck;
    public Member AsMember(int id) => new Member(id, enemyName, "Enemy", TeamType.Enemies, Stats);
    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public GameObject Prefab => prefab;
    
    public IStats Stats => new StatAddends { ResourceTypes = new IResourceType[] {resourceType} }
        .With(StatType.MaxHP, maxHp)
        .With(StatType.Damagability, 1f);
}
