using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class QualityAssurance
{
    private class ValidationResult
    {
        public string ItemName { get; }
        public List<string> Issues { get; }
        public bool IsValid => Issues.None();

        public ValidationResult(string itemName, List<string> issues)
        {
            ItemName = itemName;
            Issues = issues;
        }

        public override string ToString() => $"{ItemName} issues: {string.Join(", ", Issues)}";
    }
    
    [MenuItem("Neon/QA/Run Full Content QA")]
    public static void Go()
    {
        Log.Info("QA - Started");
        QaAllEnemies();
        QaAllCards();
    }

    private static void QaAllEnemies()
    {
        var enemies = ScriptableExtensions.GetAllInstances<Enemy>();
        var badEnemies = new List<ValidationResult>();
        var numEnemiesCount = enemies.Length;
        foreach (var e in enemies)
        {
            if (!e.IsCurrentlyWorking) continue;
            
            var issues = new List<string>();
            if (!e.IsReadyForPlay) 
                issues.Add($"Broken Enemy: {e.EnemyName} is not ReadyForPlay");
            ValidateEnemyPrefab(e, issues);
            if (issues.Any())
                badEnemies.Add(new ValidationResult(e.EnemyName, issues));
        }

        var qaPassed = badEnemies.None();
        var qaResultTerm = qaPassed ? "Passed" : "Failed - See Details Below";
        Log.Info($"--------------------------------------------------------------");
        Log.InfoOrError($"QA - {qaResultTerm}", !qaPassed);
        Log.InfoOrError($"QA - Enemies: {numEnemiesCount - badEnemies.Count} out of {numEnemiesCount} passed inspection.", badEnemies.Any());
        badEnemies.ForEach(e => Log.Error($"{e}"));
        Log.Info($"--------------------------------------------------------------");
    }

    private static void QaAllCards()
    {
        var cards = ScriptableExtensions.GetAllInstances<CardType>();
        foreach (var card in cards)
        {
            if (string.Equals(card.Cost.ResourceType?.Name, "PrimaryResource"))
                Log.Error($"Card: {card.Name} has a resource cost of primary resource");
        }
    }

    private static void ValidateEnemyPrefab(Enemy e, List<string> issues)
    {
        if (!e.Prefab)
            return;

        var enemyName = e.EnemyName;
        var obj = (GameObject)PrefabUtility.InstantiatePrefab(e.Prefab);
        
        var stealth = obj.GetComponentInChildren<StealthTransparency>();
        var stealth2 = obj.GetComponentInChildren<CharacterCreatorStealthTransparency>();
        if (stealth == null && stealth2 == null) 
            Log.Info($"{enemyName} is missing a Stealth Transparency");

        if (stealth2 != null)
            if (stealth2.viewer == null)
                issues.Add($"{enemyName}'s {nameof(CharacterCreatorStealthTransparency)} {nameof(CharacterCreatorStealthTransparency.viewer)} binding is null");
        
        var shield = obj.GetComponentInChildren<ShieldVisual>();
        if (shield == null) 
            issues.Add($"{enemyName} is missing a {nameof(ShieldVisual)}");

        var highlighter = obj.GetComponentInChildren<MemberHighlighter>();
        if (highlighter == null) 
            issues.Add($"{enemyName} is missing a {nameof(MemberHighlighter)}");

        var tauntEffect = obj.GetComponentInChildren<TauntEffect>();
        if (tauntEffect == null) 
            issues.Add($"{enemyName} is missing a {nameof(TauntEffect)}");

        var talkingCharacter = obj.GetComponentInChildren<TalkingCharacter>();
        if (talkingCharacter != null && talkingCharacter.character == null)
            issues.Add($"{enemyName}'s {nameof(TalkingCharacter)} {nameof(TalkingCharacter.character)} binding is null");

        Object.DestroyImmediate(obj);
    }
}
