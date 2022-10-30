using System.Linq;
using UnityEditor;
using UnityEngine;

public class RareContentSummaryEditor
{
    [MenuItem("Neon/Show All Rare Content")]
    public static void Go()
    {
        var cardRarities = new[] {Rarity.Rare, Rarity.Epic}.ToHashSet();
        var cards = ScriptableExtensions.GetAllInstances<CardType>()
            .Where(t => t.IncludeInPools && cardRarities.Contains(t.Rarity))
            .Select(t => $"Card: {t.Name} - {t.GetArchetypeKey()}");
        
        var tiers = new[] {EnemyTier.Elite, EnemyTier.Boss}.ToHashSet();
        var enemies = ScriptableExtensions.GetAllInstances<Enemy>()
            .Where(e => e.IsCurrentlyWorking && tiers.Contains(e.Tier))
            .Select(e => $"Enemy: {e.enemyName} - {e.name}");
        
        ((ListDisplayWindow)EditorWindow.GetWindow(typeof(ListDisplayWindow)))
            .Initialized("Rare Content", "Content", cards.Concat(enemies).ToArray())
            .Show();
        GUIUtility.ExitGUI();
    }
}
