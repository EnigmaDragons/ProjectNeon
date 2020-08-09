using UnityEngine;

public sealed class CostResourceEffect : Effect
{
    private int _cost;
    private string _resourceType;

    public CostResourceEffect(int cost, string resourceType)
    {
        _cost = cost;
    }

    public void Apply(Member source, Target target)
    {
        Debug.Log("Resource :" + source.State[new InMemoryResourceType { Name = _resourceType }]);
        source.State.SpendPrimaryResource(_cost);
    }
}
