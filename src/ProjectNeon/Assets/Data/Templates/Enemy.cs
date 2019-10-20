using UnityEngine;

public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private int powerLevel;
    [SerializeField] private GameObject prefab;
    
    public Deck Deck => deck;
    public Member AsMember(int id) => new Member(id, enemyName, "Enemy", TeamType.Enemies, new StatAddends());
    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public GameObject Prefab => prefab;
}
