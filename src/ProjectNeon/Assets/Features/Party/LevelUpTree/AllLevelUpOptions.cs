using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Level Up Options")]
public class AllLevelUpOptions : ScriptableObject
{
    private Dictionary<int, HeroLevelUpOption> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public HeroLevelUpOption[] LevelUps; //Unity Collection Readonly

    public Dictionary<int, HeroLevelUpOption> GetMap() => _map ??= LevelUps.ToDictionary(x => x.id, x => x);
    public Maybe<HeroLevelUpOption> GetLevelUpPerkById(int id) => GetMap().ValueOrMaybe(id);
}