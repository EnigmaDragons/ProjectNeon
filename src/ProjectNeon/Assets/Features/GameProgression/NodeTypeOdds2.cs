using System.Collections.Generic;
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
    [SerializeField] private float[] storyEventChances;
    [SerializeField] private float[] clinicChances;

    public IEnumerable<MapGenerationRule3> GenerateMapRules() => new MapGenerationRule3[]
        {
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Unknown, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Start, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Boss, new [] { 0f }),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Combat, combatChances),
            new AdditionalNodeChoice(MapNodeType.Combat, combat2Chances, 1),
            new AdditionalNodeChoice(MapNodeType.Combat, combat3Chances, 2),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Elite, eliteCombatChances),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.CardShop, cardShopChances),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.GearShop, gearShopChances),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.StoryEvent, storyEventChances),
            new PercentChanceBasedOnHowLongItsBeenSinceLastTimeYouHaveDoneIt(MapNodeType.Clinic, clinicChances),
        };
}