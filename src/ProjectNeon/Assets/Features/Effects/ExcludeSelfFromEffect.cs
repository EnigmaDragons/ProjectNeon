using UnityEngine;
using UnityEditor;
using System.Linq;

public class ExcludeSelfFromEffect : Effect
{
    private Effect _origin;

    public ExcludeSelfFromEffect(Effect origin) 
    {
        _origin = origin;
    }

    public void Apply(Member source, Target target)
    {
        _origin.Apply(source, new Multiple(target.Members.Except(source).ToArray()));
    }
}