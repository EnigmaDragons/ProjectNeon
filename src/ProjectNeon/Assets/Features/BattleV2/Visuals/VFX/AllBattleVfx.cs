using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Battle VFX")]
public class AllBattleVfx : ScriptableObject
{
    [SerializeField] public BattleVFX[] allFx;

    public Dictionary<string, BattleVFX> ByName => allFx.ToDictionary(f => f.EffectName, f => f);
}
