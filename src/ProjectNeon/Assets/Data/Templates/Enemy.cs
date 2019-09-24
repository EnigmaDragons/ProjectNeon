using UnityEngine;

/**
 * @todo #55:30min Enemy class should rename it to something like AICharacter,
 *  and use it to decorate Character, but with TurnAI provided, since current
 *  Enemy and Character classes are too similar. So Enemy instances would be
 *  composed of a Character (with stats and all character data) and an TurnAI
 *  instance (which represents the AI behavior for this character)
 */
public class Enemy : ScriptableObject
{
    [SerializeField]
    public string enemyName;

    [SerializeField]
    private Card[] deck;

    [SerializeField]
    public TurnAI turn;

    [SerializeField]
    public Stats stats;

    [SerializeField]
    public int powerLevel;

    public Sprite image;

}
