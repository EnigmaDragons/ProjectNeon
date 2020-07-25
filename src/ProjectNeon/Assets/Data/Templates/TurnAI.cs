using UnityEngine;

public abstract class TurnAI : ScriptableObject
{
    public abstract IPlayedCard Play(int memberId);
}
