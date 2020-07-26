using UnityEngine;

public class CardActionsData : ScriptableObject
{
    public CardActionV2[] Actions = new CardActionV2[0];
    
    public void Play(Member source, Target target, Group group, Scope scope, int amountPaid)
        => Actions.ForEach(x => x.Play(source, target, group, scope, amountPaid));
}