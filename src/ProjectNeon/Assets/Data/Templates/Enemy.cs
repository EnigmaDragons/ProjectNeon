using UnityEngine;

public class Enemy : ScriptableObject
{
    [SerializeField]
    private string enemyName;

    [SerializeField]
    private Card[] deck;

    [SerializeField]
    private TurnAI turn;

    [SerializeField]
    private Stats stats;

    [SerializeField]
    public int powerLevel;

    public Sprite image;

}
