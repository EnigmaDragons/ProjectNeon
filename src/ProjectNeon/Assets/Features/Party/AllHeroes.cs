using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Heroes")]
public class AllHeroes : ScriptableObject
{
    [SerializeField] private BaseHero noHero;
    private Dictionary<int, BaseHero> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public BaseHero[] Heroes; //Unity Collection Readonly

    public Dictionary<int, BaseHero> GetMap() => _map ??= Heroes.ToDictionary(x => x.id, x => x);
    public Maybe<BaseHero> GetHeroById(int id) => GetMap().ValueOrMaybe(id);
    public BaseHero GetHeroByIdOrDefault(int id) => GetMap().ValueOrMaybe(id).Select(x => x, () => noHero);
}
