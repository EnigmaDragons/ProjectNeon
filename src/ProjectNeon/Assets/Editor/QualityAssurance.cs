using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using I2.Loc;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

    [MenuItem("Neon/QA/Run Full Content QA %#_Q")]
    public static bool Go()
    {
        ErrorReport.DisableDuringQa();

        Log.Info("QA - Started");

        var (enemyCount, enemyFailures) = QaAllEnemies();
        var (cardCount, cardFailures) = QaAllCards();
        var (reactionCount, reactionFailures) = QaAllReactions();
        var (heroCount, heroFailures) = QaAllHeroes();
        var (cutsceneCount, cutsceneFailures) = QaAllCutscenes();
        var (prefabCount, prefabFailures) = QaSpecificPrefabs();
        var (encounterCount, encounterFailures) = QaAllSpecificEncounterSegments();
        var (adventureCount, adventureFailures) = QaAdventures();
        var (stageSegmentCount, stageSegmentFailures) = QaStageSegments();
        var (equipmentCount, equipmentFailures) = QaAllEquipments();

        var qaPassed 
            = enemyFailures.None() 
            && cardFailures.None() 
            && reactionFailures.None() 
            && heroFailures.None() 
            && cutsceneFailures.None() 
            && prefabFailures.None() 
            && encounterFailures.None() 
            && adventureFailures.None()
            && stageSegmentFailures.None()
            && equipmentFailures.None();
        var qaResultTerm = qaPassed ? "Passed" : "Failed - See Details Below";
        Log.Info("--------------------------------------------------------------");
        Log.InfoOrError($"QA - {qaResultTerm}", !qaPassed);
        LogReport("Enemies", enemyCount, enemyFailures);
        LogReport("Cards", cardCount, cardFailures);
        LogReport("Reactions", reactionCount, reactionFailures);
        LogReport("Heroes", heroCount, heroFailures);
        LogReport("Cutscenes", cutsceneCount, cutsceneFailures);
        LogReport("Prefabs", prefabCount, prefabFailures);
        LogReport("Encounters", encounterCount, encounterFailures);
        LogReport("Adventures", adventureCount, adventureFailures);
        LogReport("StageSegments", stageSegmentCount, stageSegmentFailures);
        LogReport("Equipment", equipmentCount, equipmentFailures);
        Log.Info("--------------------------------------------------------------");

        ErrorReport.ReenableAfterQa();
        return qaPassed;
    }
    
    private static void LogReport(string qaCategory, int itemCount, List<ValidationResult> failures, bool warnInsteadOfError = false)
    {
        if (warnInsteadOfError)
        {
            Log.InfoOrWarn($"QA - {qaCategory}: {itemCount - failures.Count} out of {itemCount} passed inspection.",
                failures.Any());
            
            failures.ForEach(e => Log.Warn($"{e}")); 
        }
        else
        {
            Log.InfoOrError($"QA - {qaCategory}: {itemCount - failures.Count} out of {itemCount} passed inspection.",
                failures.Any());
        
            failures.ForEach(e => Log.Error($"{e}")); 
        }
    }

    private static int CCount(string haystack, string needle)
    {
        return haystack.Split(new[] {needle}, StringSplitOptions.None).Length - 1;
    }

    private static (int, List<ValidationResult>) QaSpecificPrefabs()
    {
        var badItems = new List<ValidationResult>();
        var deckBuilder = QaDeckBuilderPrefab();
        if (!deckBuilder.IsValid)
            badItems.Add(deckBuilder);
        return (1, badItems);
    }

    private static (int stageSegmentCount, List<ValidationResult> stageSegmentFailures) QaStageSegments()
    {
        var stageSegments = ScriptableExtensions.GetAllInstances<StageSegment>();
        var byId = new Dictionary<int, List<StageSegment>>();
        foreach (var s in stageSegments)
        {
            var id = s.Id;
            if (byId.TryGetValue(id, out var list))
                list.Add(s);
            else
                byId[id] = new List<StageSegment> { s };
        }

        var failures = byId
            .Where(i => i.Value.Count > 1)
            .SelectMany(i =>
                i.Value.Select(s => new ValidationResult(s.name, new List<string> { $"Has Duplicate Id {i.Key}" })))
            .ToList();

        return (stageSegments.Length, failures);
    }
    
    private static (int adventureCount, List<ValidationResult> adventureFailures) QaAdventures()
    {
        var adventures = ScriptableExtensions.GetAllInstances<Library>().SelectMany(l => l.UnlockedAdventures).ToArray();
        var badItems = new List<ValidationResult>();
        var numItems = adventures.Length;
        foreach (var a in adventures)
        {
            var issues = new List<string>();
            var stages = a.StagesV5;
            for (var i = 0; i < stages.Length; i++)
            {
                var stage = stages[i];
                var segmentCount = stage.SegmentCount;
                for (var seg = 0; seg < segmentCount; seg++)
                {
                    var mainSegment = stage.Segments[seg];
                    var autoStarts = mainSegment.ShouldAutoStart;
                    if (autoStarts)
                    {
                        if (stage.MaybeSecondarySegments.Length > seg && stage.MaybeSecondarySegments[seg] != null)
                            issues.Add($"Has Unreachable Secondary Segment - Index {seg}");
                        if (stage.MaybeStorySegments.Length > seg && stage.MaybeStorySegments[seg] != null)
                            issues.Add($"Has Unreachable Story Segment - Index {seg}");
                    }
                }
            }
            if (issues.Any())
                badItems.Add(new ValidationResult(a.name, issues));
        }

        return (numItems, badItems);
    }
    
    private static ValidationResult QaDeckBuilderPrefab()
    {
        var issues = new List<string>();
        var assets = AssetDatabase.FindAssets("t:prefab", new[] {"Assets/Prefabs/GameViewsV5"})
            .Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(x)))
            .Where(x => x.name.Equals("DeckBuilderUI-V5")).ToArray();
        if (assets.None())
            issues.Add("No DeckBuilder V5 Prefab Found");
        else
        {
            var obj = (GameObject)PrefabUtility.InstantiatePrefab(assets[0]);
            obj.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                var canvas = obj.transform.Find("DeckBuilderCanvas");
                if (canvas == null)
                    issues.Add("Count Not Find DeckBuilder Canvas");
                else if (canvas.gameObject.activeSelf)
                    issues.Add("Deck Builder Canvas must be turned off in the Prefab, but was on");
            }
            catch (Exception)
            {
            }
            SafeDestroyImmediate(obj);
        }
        
        return new ValidationResult("DeckBuilder-V5-Prefab", issues);
    }

    private static void SafeDestroyImmediate(GameObject obj)
    {
        try
        {
            Object.DestroyImmediate(obj);
        }
        catch (Exception)
        {
        }
    }
    
    private static (int, List<ValidationResult>) QaAllCutscenes()
    {
        var cutscenes = ScriptableExtensions.GetAllInstances<Cutscene>();
        var badItems = new List<ValidationResult>();
        var numItemsCount = cutscenes.Length;
        foreach (var c in cutscenes)
        {
            if (c.IsObsolete)
                continue;
            
            var issues = new List<string>();
            if (c.Setting is GameObjectSetting setting && setting.Battlefield == null)
                issues.Add($"Cutscene {c.name} has a Null Game Object Setting");
            for (var i = 0; i < c.Segments.Length; i++)
            {
                var s = c.Segments[i];
                if (s.SegmentType == CutsceneSegmentType.Choice && !s.StoryEvent.InCutscene)
                    issues.Add($"Cutscene Segment Choice not marked as In Cutscene {c.name}");
                
                if (s.SegmentType != CutsceneSegmentType.DialogueLine && s.SegmentType != CutsceneSegmentType.NarratorLine && s.SegmentType != CutsceneSegmentType.PlayerLine)
                    continue;
                var text = s.Term.ToEnglish();
                if (text == null)
                {
                    issues.Add($"Cutscene Segment Null Text: {c.name}");
                }
                else
                {
                    var numberOfLineBreaks = CCount(text.Replace("\n\n", "\n"), "\n");
                    var textEffectiveLength = text.Length + numberOfLineBreaks * 77;
                    if (textEffectiveLength > 252)
                        Log.Warn(
                            $"Cutscene Segment is a little long: {c.name} - Segment {i}. More than 252 Effective Characters. (Linebreaks count at 77, a pessimistic value)");
                    if (textEffectiveLength > 385)
                        issues.Add($"Cutscene Segment Too Long: {c.name} - Segment {i}");
                }
            }
            if (issues.Any())
                badItems.Add(new ValidationResult(c.name, issues));
        }
        
        return (numItemsCount, badItems);
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
                badItems.Add(new ValidationResult(h.name, issues));
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
                issues.Add($"Broken Enemy: {e.enemyName} is not ReadyForPlay");
            if (e.MaterialType == MemberMaterialType.Unknown)
                issues.Add($"Enemy {e.enemyName} does not have a Material Type set.");
            if (e.stageDetails.Any(x => x.powerLevel == 0))
                issues.Add($"Enemy {e.enemyName} - {e.name} does not have a Power Level set for one of their stages.");
            ValidateEnemyPrefab(e, issues);
            if (issues.Any())
                badEnemies.Add(new ValidationResult(e.enemyName, issues));
        }
        
        return (numEnemiesCount, badEnemies);
    }

    private static (int, List<ValidationResult>) QaAllSpecificEncounterSegments()
    {
        var encounters = ScriptableExtensions.GetAllInstances<SpecificEncounterSegment>();
        var badEncounters = new List<ValidationResult>();
        var numEncounters = encounters.Length;
        foreach (var e in encounters)
        {
            var issues = new List<string>();
            if (e.Battlefield == null)
                issues.Add($"Specific Encounter Segment {e.name} has no Battlefield Set");
            if (issues.Any())
                badEncounters.Add(new ValidationResult(e.name, issues));
        }

        return (numEncounters, badEncounters);
    }

    private static void ValidateEnemyPrefab(Enemy e, List<string> issues)
    {
        if (!e.Prefab)
            return;

        var enemyName = e.enemyName;
        var obj = (GameObject)PrefabUtility.InstantiatePrefab(e.Prefab);
        obj.hideFlags = HideFlags.HideAndDontSave;
        
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
        
        var stunnedDisabledEffect = obj.GetComponentInChildren<StunnedDisabledEffect>();
        if (stunnedDisabledEffect == null) 
            issues.Add($"{enemyName} is missing a {nameof(StunnedDisabledEffect)}");

        var talkingCharacter = obj.GetComponentInChildren<TalkingCharacter>();
        if (talkingCharacter != null && talkingCharacter.character == null)
            issues.Add($"{enemyName}'s {nameof(TalkingCharacter)} {nameof(TalkingCharacter.character)} binding is null");

        var cutsceneCharacter = obj.GetComponentInChildren<CutsceneCharacter>();
        if (cutsceneCharacter != null && cutsceneCharacter.SpeechBubble == null)
            issues.Add($"{enemyName}'s {nameof(CutsceneCharacter)} {nameof(CutsceneCharacter.SpeechBubble)} binding is null");
        
        SafeDestroyImmediate(obj);
    }

    private static (int, List<ValidationResult>) QaAllCards()
        => WithLocalization(() =>
        {
            var items = ScriptableExtensions.GetAllInstances<CardType>().Where(c => !c.IsWip).ToArray();
            var failures = new List<ValidationResult>();
            var itemCount = items.Length;
            foreach (var i in items)
            {
                try
                {
                    var issues = new List<string>();
                    if (string.IsNullOrWhiteSpace(i.NameTerm.ToEnglish()))
                        issues.Add($"Broken Card: Missing Name");
                    else if (string.IsNullOrWhiteSpace(i.LocalizedDescription(
                        new Member(1, "quality assurance", "Any Class", MemberMaterialType.Organic, TeamType.Party,
                            new StatAddends().With(StatType.Damagability, 1), BattleRole.Unknown, StatType.Attack,
                            false, false), ResourceQuantity.None)))
                        issues.Add($"Broken Card: Cannot Interpolate Description");
                    else if (i.cost == null || i.cost.RawResourceType == null)
                        issues.Add($"Broken Card: {i.Name} has Null Cost");
                    else if (i.ActionSequences.Any(e => e.CardActions == null))
                        issues.Add($"Broken Card: {i.Name} has a Null Card Action");
                    else if (!i.Archetypes.Contains("Enemy")
                             && i.actionSequences.Any(x =>
                                 x.Group == Group.Ally && (x.Scope == Scope.One || x.Scope == Scope.OneExceptSelf))
                             && i.actionSequences.Any(x =>
                                 x.Group == Group.Opponent && (x.Scope == Scope.One || x.Scope == Scope.OneExceptSelf)))
                        issues.Add($"Broken Card: {i.Name} has conflicting targeting scopes");
                    else if (i.highlightCondition.Any(x => x == null) || i.unhighlightCondition.Any(x => x == null))
                        issues.Add($"Broken Card: {i.Name} has null highlight conditions");
                    if (issues.Any())
                        failures.Add(new ValidationResult($"{i.Name} - {i.id}", issues));
                }
                catch (Exception)
                {
                    
                }
            }

            return (itemCount, failures);
        });

    private static (int, List<ValidationResult>) QaAllReactions()
        => WithLocalization(() =>
        {
            var items = ScriptableExtensions.GetAllInstances<ReactionCardType>().ToArray();
            var failures = new List<ValidationResult>();
            var itemCount = items.Length;
            foreach (var i in items)
            {
                var issues = new List<string>();
                if (string.IsNullOrWhiteSpace(i.NameTerm.ToEnglish()))
                    issues.Add($"Broken Card: Missing Name");
                else if (string.IsNullOrWhiteSpace(i.LocalizedDescription(
                             new Member(1, "quality assurance", "Any Class", MemberMaterialType.Organic, TeamType.Party,
                                 new StatAddends().With(StatType.Damagability, 1), BattleRole.Unknown, StatType.Attack,
                                 false, false), ResourceQuantity.None)))
                    issues.Add($"Broken Card: Cannot Interpolate Description");
                else if (i.Cost == null || i.Cost.ResourceType == null)
                    issues.Add($"Broken Card: {i.Name} has Null Cost");
                else if (i.ActionSequences.Any(e => e.CardActions == null))
                    issues.Add($"Broken Card: {i.Name} has a Null Card Action");
                if (issues.Any())
                    failures.Add(new ValidationResult($"{i.Name} - {i.id}", issues));
            }

            return (itemCount, failures);
        });

    private static (int, List<ValidationResult>) QaAllEquipments()
        => WithLocalization(() =>
        {
            var items = ScriptableExtensions.GetAllInstances<StaticEquipment>().Where(x => !x.IsWip).ToArray();
            var failures = new List<ValidationResult>();
            var itemCount = items.Length;
            foreach (var i in items)
            {
                var issues = new List<string>();
                if (i.BattleStartEffects.Concat(i.TurnStartEffects).Any(x
                        => (x.EffectType == EffectType.ReactWithCard || x.EffectType == EffectType.ReactWithEffect ||
                            x.EffectType == EffectType.ReactOncePerTurnWithEffect ||
                            x.EffectType == EffectType.ShowCustomTooltip)
                           && x.StatusTag != StatusTag.None
                           && (x.Id == 0 || string.IsNullOrWhiteSpace(x.StatusDetailTerm.ToEnglish()))))
                    issues.Add($"Broken Equipment: {i.Name} has missing status details");
                if (issues.Any())
                    failures.Add(new ValidationResult($"{i.Name} - {i.id}", issues));
            }

            return (itemCount, failures);
        });

    private static T WithLocalization<T>(Func<T> func)
    {
        var editorLocalizationParams = new MpZeroEditorGlobalLocalizationParams();
        LocalizationManager.ParamManagers.Add(editorLocalizationParams);
        LocalizationManager.LocalizeAll(true);
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            Log.Error($"Unexpected Exception of type {ex.GetType().Name}: {ex.Message}");
            throw;
        }
        finally
        {
            LocalizationManager.ParamManagers.Remove(editorLocalizationParams);
        }
    }

    private static Regex _specialTag = new Regex(@"{\[(.+?)]}", RegexOptions.IgnoreCase);
    private static string[] _validSpecialTags = new[] { "Originator", "PrimaryStat", "SaveGameVersion", "GameVersion", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    private static Regex _xmlTags = new Regex(@"<.+?>");
    private static Regex _validXmlTags = new Regex(@"<(b|i|\/b|\/i|\/size|\/color|color=#......|size=\d+%|sprite index=\d+|s|\/s)>");
    private static Regex _specialOpenTag = new Regex(@"{\[");
    private static Regex _specialCloseTag = new Regex("]}");
    private static Regex _invalidSpecialTag = new Regex(@"{[^\[]t:.*[^]]}", RegexOptions.IgnoreCase);
    private static Regex _formatNumbers = new Regex(@"{\[(\d)]}");

    [MenuItem("Neon/QA/QA Terms")]
    private static void QaTerms()
    {
        var (termCount, termFailures) = QaAllTerms();
        LogReport("Terms", termCount, termFailures);
        CheckMissingTerms();
    }

    [MenuItem("Neon/QA/Check Missing Terms Per Language")]
    private static void CheckMissingTerms()
    {
        var missingTermsByLanguage = new Dictionary<string, HashSet<string>>();
        var languageSources = ScriptableExtensions.GetAllInstances<LanguageSourceAsset>();
        var itemCount = 0;
        var languages = LocalizationManager.GetAllLanguages();
        foreach (LanguageSourceAsset source in languageSources)
        {
            var terms = source.mSource.mTerms.Select(x => x.Term).ToHashSet();
            itemCount = terms.Count;
            foreach (var term in terms)
            {
                for (var i = 0; i < languages.Count; i++) { 
                    var language = languages[i];
                    if (!missingTermsByLanguage.ContainsKey(language))
                        missingTermsByLanguage[language] = new HashSet<string>();
                    if (string.IsNullOrEmpty(source.SourceData.mDictionary[term].Languages[i]))
                        if (!string.IsNullOrEmpty(LocalizationManager.GetTranslation(term, overrideLanguage: "English")))
                            missingTermsByLanguage[language].Add(term);
                }
            }
        }
    
        var failures = missingTermsByLanguage.Where(x => x.Value.Count > 0)
            .Select(x => new ValidationResult($"{x.Key} - {(1 - (x.Value.Count/(float)itemCount)).ToString("P")} Complete - Missing {x.Value.Count} Terms: ", x.Value.ToList())).ToList();

        LogReport("Incomplete Language Translations - Missing Terms", languages.Count, failures, warnInsteadOfError: true);
    }

    private static (int, List<ValidationResult>) QaAllTerms()
    {
        var languageSources = ScriptableExtensions.GetAllInstances<LanguageSourceAsset>();
        var failures = new List<ValidationResult>();
        var itemCount = 0;
        foreach (LanguageSourceAsset source in languageSources)
        {
            var terms = source.mSource.mTerms.Select(x => x.Term).ToHashSet();
            itemCount = terms.Count;
            foreach (var term in terms)
            {
                var issues = new List<string>();
                var englishStr = LocalizationManager.GetTranslation(term, overrideLanguage: "English");
                if (string.IsNullOrEmpty(englishStr))
                    continue;
                var formatTags = new HashSet<int>();
                foreach (Match match in _formatNumbers.Matches(englishStr))
                    formatTags.Add(int.Parse(match.Groups[1].Value));
                foreach (var language in LocalizationManager.GetAllLanguages())
                {
                    var str = LocalizationManager.GetTranslation(term, overrideLanguage: language);
                    if (string.IsNullOrEmpty(str))
                        continue;
                    
                    var specialTags = _specialTag.Matches(str);
                    foreach (Match tag in specialTags)
                    {
                        var value = tag.Groups[1].Value;
                        if (value.StartsWith("t:"))
                        {
                            var subterm = value.Substring(2);
                            if (!terms.Contains(subterm))
                                issues.Add($"{term} ({language}) has an invalid subterm {{[t:{subterm}]}}");
                        }
                        else if (!_validSpecialTags.Contains(value))
                            Log.Error($"{term} ({language}) has an invalid special tag {{[{value}]}}");
                    }

                    var xmlTags = _xmlTags.Matches(str);
                    var xmlExceptionTerms = new HashSet<string>
                    {
                        "BattleUI/You Were Defeated",
                        "BattleUI/You Were Victorious!",
                        "Menu/Wishlist Daisy Dog"
                    };
                    foreach (Match tag in xmlTags)
                    {
                        if (!_validXmlTags.IsMatch(tag.Value) && !xmlExceptionTerms.Contains(term))
                            issues.Add($"{term} ({language}) has an invalid xml tag {tag.Value}");
                    }
                    
                    if (str.Count(x => x == '<') != str.Count(x => x == '>') && !xmlExceptionTerms.Contains(term))
                        issues.Add($"{term} ({language}) has an invalid <> tag");
                    if (_specialOpenTag.Matches(str).Count != _specialCloseTag.Matches(str).Count)
                        issues.Add($"{term} ({language}) has an invalid {{[]}} tag");
                    else if (str.Count(x => x == '{') != str.Count(x => x == '}'))
                        issues.Add($"{term} ({language}) has an invalid {{}} tag");
                    if (_invalidSpecialTag.IsMatch(str))
                        issues.Add($"{term} ({language}) has a {{[t:]}} tag with out the [] tag");
                    
                    var languageFormatTags = new HashSet<int>();
                    foreach (Match match in _formatNumbers.Matches(str))
                        languageFormatTags.Add(int.Parse(match.Groups[1].Value));
                    if (formatTags.Count != languageFormatTags.Count || formatTags.Any(x => !languageFormatTags.Contains(x)))
                        issues.Add($"{term} ({language}) has the wrong string format tags");
                }
                if (issues.Any())
                    failures.Add(new ValidationResult($"Term: {term}", issues));
            }
        }
        return (itemCount, failures);
    }
    
    private static void ValidateHeroPrefab(BaseHero h, List<string> issues)
    {
        if (!h.Body)
            return;

        var hName = h.name;
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
        
        var stunnedDisabledEffect = obj.GetComponentInChildren<StunnedDisabledEffect>();
        if (stunnedDisabledEffect == null) 
            issues.Add($"{hName} is missing a {nameof(StunnedDisabledEffect)}");

        var talkingCharacter = obj.GetComponentInChildren<TalkingCharacter>();
        if (talkingCharacter == null)
            issues.Add($"{hName} is missing a {nameof(TalkingCharacter)}");
        if (talkingCharacter != null && talkingCharacter.character == null)
            issues.Add($"{hName}'s {nameof(TalkingCharacter)} {nameof(TalkingCharacter.character)} binding is null");

        Object.DestroyImmediate(obj);
    }
}
