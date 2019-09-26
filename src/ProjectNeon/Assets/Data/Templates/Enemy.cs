using UnityEngine;

public class Enemy : ScriptableObject
{
    [SerializeField] private string enemyName;
    [SerializeField] private Deck deck;
    [SerializeField] private TurnAI ai;
    [SerializeField] private Stats stats;
    [SerializeField] private int powerLevel;
    [SerializeField] private Sprite image;

    public Deck Deck => deck;
    public Member AsMember() => new Member(TeamType.Enemies, stats);
    public TurnAI AI => ai;
    public int PowerLevel => powerLevel;
    public Sprite Image => image;
}
