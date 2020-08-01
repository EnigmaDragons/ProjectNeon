using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CardActionsData : ScriptableObject
{
    public CardActionV2[] Actions = new CardActionV2[0];
    
    public IPayloadProvider Play(Member source, Target target, Group group, Scope scope, int amountPaid)
        => new MultiplePayloads(Actions.Select(x => x.Play(source, target, group, scope, amountPaid)).ToArray());
}
