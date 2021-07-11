using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/NodeOdds2")]
public class NodeTypeOdds2 : ScriptableObject
{
    [SerializeField] private float[] combatChances;
    [SerializeField] private float[] combat2Chances;
    [SerializeField] private float[] combat3Chances;
    [SerializeField] private float[] eliteCombatChances;
    [SerializeField] private float[] cardShopChances;
    [SerializeField] private float[] gearShopChances;
    [SerializeField] private float[] gearShop2Chances;
    [SerializeField] private float[] gearShop3Chances;
    [SerializeField] private float[] storyEventChances;
    [SerializeField] private float[] clinicChances;

    public bool IsThereTravelEvent(CurrentGameMap3 map)
    {
        var lastIndexOf = map.CompletedNodes.ToArray().LastIndexOf(x => x.Type == MapNodeType.StoryEvent);
        var turnsSinceLastTimeYouDidThisNode = lastIndexOf == -1
            ? storyEventChances.Length - 1
            : Math.Min(storyEventChances.Length - 1, map.CompletedNodes.Count - lastIndexOf - 1);
        return Rng.Chance(storyEventChances[turnsSinceLastTimeYouDidThisNode]);
    }
    
    public IEnumerable<MapGenerationRule3> GenerateMapRules() => new MapGenerationRule3[]
        {
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Unknown, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Start, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Boss, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.StoryEvent, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Combat, combatChances),
            new AdditionalNodeChoice(MapNodeType.Combat, combat2Chances, 1),
            new AdditionalNodeChoice(MapNodeType.Combat, combat3Chances, 2),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Elite, eliteCombatChances),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.CardShop, cardShopChances),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.GearShop, gearShopChances),
            new AdditionalNodeChoice(MapNodeType.GearShop, gearShop2Chances, 1),
            new AdditionalNodeChoice(MapNodeType.GearShop, gearShop3Chances, 2),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Clinic, clinicChances),
        };
}