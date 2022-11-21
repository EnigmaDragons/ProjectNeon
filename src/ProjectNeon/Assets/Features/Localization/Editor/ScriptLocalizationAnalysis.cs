#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using I2.Loc;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class ScriptLocalizationAnalysis : EditorWindow
{
    /*[MenuItem("Neon/Script Localization Prefab Summary")]
    static void PrefabSummary()
    {
        var results = new List<MonoScriptLocalizationSummary>();
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        var scriptPaths = assetPaths.Where(x => x.EndsWith(".cs"));
        var prefabPaths = assetPaths.Where(x => x.EndsWith(".prefab")).ToArray();
        var dependencies = new HashSet<string>();
        foreach (var dependency in AssetDatabase.GetDependencies(prefabPaths))
            dependencies.Add(dependency);
        foreach (var scriptPath in scriptPaths)
        {
            if (!dependencies.Contains(scriptPath))
                continue;
            var type = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath).GetClass();
            if (type == null || !type.IsSubclassOf(typeof(MonoBehaviour)))
                continue;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (fields.None(x => x.FieldType == typeof(TextMeshProUGUI) || x.FieldType == typeof(Localize)))
                continue;
            results.Add(SummarizeScript(scriptPath, type, fields));
        }
        Display(results.ToArray());
    }*/

    private static string[] _excludedNamespaces = new []{"TMPro", "MoreMountains"};

    [MenuItem("Neon/Script Localization Summary")]
    static void AllSummary()
    {
        var results = new List<MonoScriptLocalizationSummary>();
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        var scriptPaths = assetPaths.Where(x => x.EndsWith(".cs"));
        foreach (var scriptPath in scriptPaths)
        {
            var type = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath).GetClass();
            if (type == null || !type.IsSubclassOf(typeof(MonoBehaviour)) || (type.Namespace != null && _excludedNamespaces.Any(x => type.Namespace.StartsWith(x))))
                continue;
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (fields.None(x => x.FieldType == _textMeshProUGuiType || x.FieldType == _textMeshProType || x.FieldType == _localizeType || x.FieldType == _tmpText))
                continue;
            results.Add(SummarizeScript(scriptPath, type, fields));
        }
        Display(results.ToArray());
    }

    private static MonoScriptLocalizationSummary SummarizeScript(string path, Type type, FieldInfo[] fields)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            var rawFileText = reader.ReadToEnd();
            return new MonoScriptLocalizationSummary
            {
                Name = type.Name,
                Fields = fields.Where(ShouldSummarizeField).Select(x => SummarizeField(x, rawFileText)).ToArray(),
                IgnoreForLocalization = Attribute.IsDefined(type, typeof(IgnoreForLocalizationAttribute))
            };
        }
    }

    private static Type _textMeshProUGuiType = typeof(TextMeshProUGUI);
    private static Type _textMeshProType = typeof(TextMeshPro);
    private static Type _tmpText = typeof(TMP_Text);
    private static Type _localizeType = typeof(Localize);
    
    private static bool ShouldSummarizeField(FieldInfo field)
        => field.FieldType == _textMeshProUGuiType || field.FieldType == _textMeshProType || field.FieldType == _localizeType || field.FieldType == _tmpText;
    
    private static FieldLocalizationSummary SummarizeField(FieldInfo field, string rawFileText)
    {
        var type = field.FieldType == typeof(Localize) 
            ? FieldLocalizationType.Localize 
            : FieldLocalizationType.TextMeshPro;
        var setterCalled = false;
        if (type == FieldLocalizationType.TextMeshPro)
            setterCalled = new Regex($"{field.Name}.text\\s?=[^=]").IsMatch(rawFileText);
        else
            setterCalled = new Regex($"{field.Name}\\.SetTerm|{field.Name}\\.SetFinalText").IsMatch(rawFileText);
        return new FieldLocalizationSummary
        {
            Type = type,
            NoLocalizationNeeded = Attribute.IsDefined(field, typeof(NoLocalizationNeededAttribute)),
            SetterCalled = setterCalled,
            Name = field.Name
        };
    }
    
    private static void Display(MonoScriptLocalizationSummary[] summaries)
    {
        var items = new List<string>();
        items.Add($"Progress: {summaries.Count(x => x.IsLocalized)}/{summaries.Length}");
        foreach (var summary in summaries.OrderBy(x => x.IgnoreForLocalization ? 2 : x.IsLocalized ? 1 : 0))
        {
            items.Add("");
            var classSummaryWord = "Incomplete";
            if (summary.IgnoreForLocalization)
                classSummaryWord = "Ignored";
            else if (summary.IsLocalized)
                classSummaryWord = "Complete";
            items.Add($"  Class: {summary.Name} - {classSummaryWord} - {summary.TotalHits}/{summary.Fields.Length}");
            foreach (var field in summary.Fields)
            {
                var summaryWord = "Not Localized";
                if (!field.SetterCalled)
                    summaryWord = "Text Never Set";
                else if (field.Type == FieldLocalizationType.Localize)
                    summaryWord = "Localized";
                else if (field.NoLocalizationNeeded)
                    summaryWord = "No Localization Needed";
                items.Add($"    Field: {field.Name} - {summaryWord}");
            }
        }
        GetWindow<ListDisplayWindow>().Initialized("Script Localization Progress", "", items.ToArray()).Show();
    }

    private class MonoScriptLocalizationSummary
    {
        public string Name;
        public FieldLocalizationSummary[] Fields;
        public bool IgnoreForLocalization;
        
        public bool IsLocalized => IgnoreForLocalization || Fields.All(x => x.IsLocalized);
        public int TotalHits => Fields.Count(x => x.IsLocalized);
    }

    private class FieldLocalizationSummary
    {
        public FieldLocalizationType Type;
        public bool NoLocalizationNeeded;
        public bool SetterCalled;
        public string Name;
        
        public bool IsLocalized => Type == FieldLocalizationType.Localize || NoLocalizationNeeded || !SetterCalled;
    }

    private enum FieldLocalizationType
    {
        TextMeshPro,
        Localize
    }
}

#endif