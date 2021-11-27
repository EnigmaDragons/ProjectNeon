using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Level Up Options")]
public class AllLevelUpOptions : ScriptableObject
{
    private Dictionary<int, StaticHeroLevelUpOption> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public StaticHeroLevelUpOption[] LevelUps; //Unity Collection Readonly

    public Dictionary<int, StaticHeroLevelUpOption> GetMap() => _map ??= LevelUps.ToDictionary(x => x.id, x => x);
    public Maybe<StaticHeroLevelUpOption> GetLevelUpPerkById(int id) => GetMap().ValueOrMaybe(id);
}