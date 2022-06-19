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
    public static bool Go()
    {
        ErrorReport.DisableDuringQa();
        
        Log.Info("QA - Started");

        var (enemyCount, enemyFailures) = QaAllEnemies();
        var (cardCount, cardFailures) = QaAllCards();
        var (heroCount, heroFailures) = QaAllHeroes();

        var qaPassed = enemyFailures.None() && cardFailures.None();
        var qaResultTerm = qaPassed ? "Passed" : "Failed - See Details Below";
        Log.Info($"--------------------------------------------------------------");
        Log.InfoOrError($"QA - {qaResultTerm}", !qaPassed);
        Log.InfoOrError($"QA - Enemies: {enemyCount - enemyFailures.Count} out of {enemyCount} passed inspection.", enemyFailures.Any());
        enemyFailures.ForEach(e => Log.Error($"{e}"));
        Log.InfoOrError($"QA - Cards: {cardCount - cardFailures.Count} out of {cardCount} passed inspection.", cardFailures.Any());
        cardFailures.ForEach(e => Log.Error($"{e}"));
        Log.InfoOrError($"QA - Heroes: {heroCount - heroFailures.Count} out of {heroCount} passed inspection.", heroFailures.Any());
        heroFailures.ForEach(e => Log.Error($"{e}"));
        Log.Info($"--------------------------------------------------------------");
        
        ErrorReport.ReenableAfterQa();
        return qaPassed;
    }

    private static (int, List<ValidationResult>) QaAllHeroes()
    {
        var heroes = ScriptableExtensions.GetAllInstances<BaseHero>();
        var badItems = new List<ValidationResult>();
        var numItemsCount = heroes.Length;
        foreach (var h in heroes)
        {
            var issues = new List<string>();
            ValidateHeroPrefab(h, issues);
            if (issues.Any())
                badItems.Add(new ValidationResult(h.Name, issues));
        }
        
        return (numItemsCount, badItems);
    }
    
    private static (int, List<ValidationResult>) QaAllEnemies()
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
        
        return (numEnemiesCount, badEnemies);
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

        var deathPresenter = obj.GetComponentInChildren<DeathPresenter>();
        if (deathPresenter == null)
            issues.Add($"{enemyName} is missing a {nameof(DeathPresenter)}");

        if (deathPresenter != null && deathPresenter.sprite == null && deathPresenter.characterViewer == null)
            issues.Add($"{enemyName}'s {nameof(DeathPresenter)} has no Sprite or Character Viewer binding");

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

        var cutsceneCharacter = obj.GetComponentInChildren<CutsceneCharacter>();
        if (cutsceneCharacter != null && cutsceneCharacter.SpeechBubble == null)
            issues.Add($"{enemyName}'s {nameof(CutsceneCharacter)} {nameof(CutsceneCharacter.SpeechBubble)} binding is null");
        
        Object.DestroyImmediate(obj);
    }

    private static (int, List<ValidationResult>) QaAllCards()
    {
        var items = ScriptableExtensions.GetAllInstances<CardType>();
        var failures = new List<ValidationResult>();
        var itemCount = items.Length;
        foreach (var i in items)
        {
            var issues = new List<string>();
            if (i.cost.RawResourceType == null) 
                issues.Add($"Broken Card: {i.Name} has Null Cost");
            if (issues.Any())
                failures.Add(new ValidationResult($"{i.Name} - {i.id}", issues));
        }
        
        return (itemCount, failures);
    }
    
    private static void ValidateHeroPrefab(BaseHero h, List<string> issues)
    {
        if (!h.Body)
            return;

        var hName = h.Name;
        var obj = (GameObject)PrefabUtility.InstantiatePrefab(h.Body);
        
        var stealth = obj.GetComponentInChildren<CharacterCreatorStealthTransparency>();
        if (stealth == null) 
            issues.Add($"{hName} is missing a Stealth Transparency");
        if (stealth != null)
            if (stealth.viewer == null)
                issues.Add($"{hName}'s {nameof(CharacterCreatorStealthTransparency)} {nameof(CharacterCreatorStealthTransparency.viewer)} binding is null");

        var deathPresenter = obj.GetComponentInChildren<DeathPresenter>();
        if (deathPresenter == null)
            issues.Add($"{hName} is missing a {nameof(DeathPresenter)}");
        if (deathPresenter != null && deathPresenter.sprite == null && deathPresenter.characterViewer == null)
            issues.Add($"{hName}'s {nameof(DeathPresenter)} has no Sprite or Character Viewer binding");

        var shield = obj.GetComponentInChildren<ShieldVisual>();
        if (shield == null) 
            issues.Add($"{hName} is missing a {nameof(ShieldVisual)}");

        var highlighter = obj.GetComponentInChildren<MemberHighlighter>();
        if (highlighter == null) 
            issues.Add($"{hName} is missing a {nameof(MemberHighlighter)}");

        var tauntEffect = obj.GetComponentInChildren<TauntEffect>();
        if (tauntEffect == null) 
            issues.Add($"{hName} is missing a {nameof(TauntEffect)}");

        var talkingCharacter = obj.GetComponentInChildren<TalkingCharacter>();
        if (talkingCharacter == null)
            issues.Add($"{hName} is missing a {nameof(TalkingCharacter)}");
        if (talkingCharacter != null && talkingCharacter.character == null)
            issues.Add($"{hName}'s {nameof(TalkingCharacter)} {nameof(TalkingCharacter.character)} binding is null");

        Object.DestroyImmediate(obj);
    }
}
