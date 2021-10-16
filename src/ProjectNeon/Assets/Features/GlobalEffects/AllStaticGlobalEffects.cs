using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GlobalEffects/AllStaticGlobalEffects")]
public class AllStaticGlobalEffects : ScriptableObject
{
    private Dictionary<int, GlobalEffect> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public StaticGlobalEffect[] Effects; //Unity Collection Readonly

    public Dictionary<int, GlobalEffect> GetMap() => _map ??= Effects.ToDictionary(x => x.id, x => (GlobalEffect)x);
    public Maybe<GlobalEffect> GetEffectById(int id) => GetMap().ValueOrMaybe(id);
}
