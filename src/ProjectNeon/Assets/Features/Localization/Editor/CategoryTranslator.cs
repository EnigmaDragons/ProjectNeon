#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEditor;
using UnityEngine;

public class CategoryTranslator : EditorWindow
{
    [MenuItem("Neon/Localization/Get Language Source")]
    public static LanguageSourceData GetSource()
    {
        var s = AssetDatabase.LoadAssetAtPath<LanguageSourceAsset>("Assets/Resources/I2Languages.asset");
        return s.mSource;
    }

    [MenuItem("Neon/Localization/Open Category Translator")]
    public static void SelectGameObjects()
    {
        _shouldRefreshCategory = false;
        _isTranslating = false;
        _lastTranslatedTerm = "";
        _categoryTranslatedCount = -1;
        _categoryUntranslatedCount = -1;
        _activeTranslationCount = 0;
        _autoTranslateAll = false;

        _source = GetSource();
        GetWindow(typeof(CategoryTranslator)).Show();
    }

    private static bool _shouldRefreshCategory = false;
    private static bool _isTranslating;
    private static string _lastTranslatedTerm;
    private static int _categoryUntranslatedCount = -1;
    private static int _categoryTranslatedCount = -1;
    private static int _activeTranslationCount = 0;
    private static LanguageSourceData _source;
    private static bool _autoTranslateAll = false;
    private static string _autoTranslateCategory = "";
    private static long _lastLogTime = 0;
    
    private string _categoryTranslationSummary = "Category Not Initialized";
    private string _termString;
    private string _categoryString;

    private const bool ShouldDebug = false;
    
    private struct TranslationState<T>
    {
        public bool IsTranslated;
        public T Data;
    }

    static void MoveNext(string categoryString)
    {
        if (string.IsNullOrWhiteSpace(categoryString))
            return;
        
        _shouldRefreshCategory = true;
        _source ??= GetSource();
        if (ShouldDebug)
            Log.Info($" Translate Move Next - Is Translating {_isTranslating} Auto {_autoTranslateAll} Active {_activeTranslationCount}");
        if (!_isTranslating && _autoTranslateAll && _activeTranslationCount < 1)
        {
            _source.mTerms
                .Where(t => t.TermType == eTermType.Text)
                .Where(t => t.Term.StartsWith($"{categoryString}/"))
                .Where(t => t.Languages.Any(string.IsNullOrWhiteSpace))
                .Where(t => !t.Term.Equals(_lastTranslatedTerm))
                .Take(1)
                .FirstAsMaybe()
                .ExecuteIfPresentOrElse(t =>
                {
                    _isTranslating = true;
                    TranslateLanguage(_source, t.Term, t);
                }, () => { _autoTranslateAll = false; });
        }
        else
        {
            if (ShouldDebug)
                Log.Info($"No Translate Move Next - Is Translating {_isTranslating} Auto {_autoTranslateAll} Active {_activeTranslationCount}");

            var lastTranslatedCount = _categoryTranslatedCount;
            RefreshCategory(categoryString);
            if (lastTranslatedCount != _categoryTranslatedCount)
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var durationMs = now - _lastLogTime;
                _lastLogTime = now;
                Log.Info($"Translator: {categoryString} - Translated: {_categoryTranslatedCount} Untranslated: {_categoryUntranslatedCount} - {durationMs}ms");
            }
        }
    }

    static Dictionary<string, TranslationState<TermData>> RefreshCategory(string categoryString)
    {
        _shouldRefreshCategory = false;
        var textTermsInCategory = _source.mTerms
            .Where(t => t.TermType == eTermType.Text)
            .Where(t => t.Term.StartsWith($"{categoryString}/"))
            .ToDictionary(t => t.Term, t => new TranslationState<TermData> { IsTranslated = t.Languages.All(l => !string.IsNullOrWhiteSpace(l)), Data = t });
        _categoryTranslatedCount = textTermsInCategory.Count(t => t.Value.IsTranslated);
        _categoryUntranslatedCount = textTermsInCategory.Count(t => !t.Value.IsTranslated);
        return textTermsInCategory;
    }
    
    void OnGUI()
    {
        _source ??= GetSource();
        
        GUI.enabled = true;
        var translatingString = _autoTranslateAll ? "Auto-Translating" : "Translating";
        EditorGUILayout.LabelField(_isTranslating || _autoTranslateAll ? $"Status: {translatingString}... {_termString}" : "Status :Awaiting Command");
        DrawUILine();
        
        EditorGUILayout.LabelField($"Active Translation Items: {_activeTranslationCount}");
        EditorGUILayout.LabelField($"Last Translated... {_lastTranslatedTerm}");
        DrawUILine();
        
        
        if (!string.IsNullOrWhiteSpace(_categoryString))
            _categoryTranslationSummary = $"{_categoryString} - Translated: {_categoryTranslatedCount} Untranslated: {_categoryUntranslatedCount}"; 
        EditorGUILayout.LabelField(_categoryTranslationSummary);
        Rect r = EditorGUILayout.BeginVertical();
        var progress = _categoryTranslatedCount > 0
            ? (float)_categoryTranslatedCount / (_categoryTranslatedCount + _categoryUntranslatedCount)
            : 0f;
        EditorGUI.ProgressBar(r, progress , "Category Translation Progress");
        GUILayout.Space(16);
        EditorGUILayout.EndVertical();
        DrawUILine();
        
        if (_isTranslating || _autoTranslateAll)
            GUI.enabled = false;
        
        _categoryString = EditorGUILayout.TextField(_categoryString);
        if (GUILayout.Button("Refresh Category") || _shouldRefreshCategory)
        {
            var termSummary = RefreshCategory(_categoryString).Select(kvp => $"{kvp.Key} - Translated {kvp.Value.IsTranslated}").ToArray();
            GetWindow<ListDisplayWindow>()
                .Initialized($"{_categoryString} Terms", "Term", termSummary)
                .Show();
        }

        if (string.IsNullOrWhiteSpace(_categoryString))
            GUI.enabled = false;
        if (GUILayout.Button("Auto-Translate Next Term")) 
            MoveNext(_categoryString);

        GUI.enabled = GUI.enabled && !_autoTranslateAll;
        if (GUILayout.Button("Auto-Translate All"))
        {
            _lastLogTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _autoTranslateAll = true;
            _autoTranslateCategory = _categoryString;
            MoveNext(_autoTranslateCategory);
        }

        GUI.enabled = _autoTranslateAll;
        if (GUILayout.Button("Stop Auto-Translate"))
        {
            _autoTranslateAll = false;
            _autoTranslateCategory = "";
        }

        GUI.enabled = true;
    }
    
    static void TranslateLanguage(LanguageSourceData source, string Key, TermData termdata)
    {
        Localize localizeCmp = null;
        string mainText = localizeCmp == null ? LanguageSourceData.GetKeyFromFullTerm(Key) : localizeCmp.GetMainTargetsText();

        for (int i = 0; i < source.mLanguages.Count; ++i)
            if (source.mLanguages[i].IsEnabled() && string.IsNullOrEmpty(termdata.Languages[i]))
            {
                var langIdx = i;
                var term = termdata;
                var i2source = source;
                Translate(source, mainText, ref termdata, source.mLanguages[i].Code,
                    (translation, error) =>
                    {
                        _isTranslating = false;
                        _activeTranslationCount--;
                        if (error != null)
                            Log.Error(error);
                        else
                        if (translation != null)
                        {
                            term.Languages[langIdx] = translation; //SetTranslation(langIdx, translation);
                            i2source.Editor_SetDirty();
                            _lastTranslatedTerm = Key;
                        }
                        MoveNext(_autoTranslateCategory);
                    }, null);
            }
    }
    
    static void Translate (LanguageSourceData source, string Key, ref TermData termdata, string TargetLanguageCode, GoogleTranslation.fnOnTranslated onTranslated, string overrideSpecialization )
    {
#if UNITY_WEBPLAYER
			ShowError ("Contacting google translation is not yet supported on WebPlayer" );
#else
        
        if (!GoogleTranslation.CanTranslate())
            LocalizationManager.InitializeIfNeeded();

        if (!GoogleTranslation.CanTranslate())
        {
            Log.Error("WebService is not set correctly or needs to be reinstalled");
            return;
        }

        // Translate first language that has something
        // If no language found, translation will fallback to autodetect language from key

        string sourceCode, sourceText;
        FindTranslationSource(source, Key, termdata, TargetLanguageCode, overrideSpecialization, out sourceText, out sourceCode );
        _activeTranslationCount++;
        GoogleTranslation.Translate( sourceText, sourceCode, TargetLanguageCode, onTranslated );
			
#endif
    }
    
    static void FindTranslationSource(LanguageSourceData source, string Key, TermData termdata, string TargetLanguageCode, string forceSpecialization, out string sourceText, out string sourceLanguageCode )
    {
        sourceLanguageCode = "auto";
        sourceText = Key;
			
        for (int i = 0, imax = termdata.Languages.Length; i < imax; ++i)
        {
            if (source.mLanguages[i].IsEnabled() && !string.IsNullOrEmpty(termdata.Languages[i]))
            {
                sourceText = forceSpecialization==null ? termdata.Languages[i] : termdata.GetTranslation(i, forceSpecialization, editMode:true);
                if (source.mLanguages[i].Code != TargetLanguageCode)
                {
                    sourceLanguageCode = source.mLanguages[i].Code;
                    return;
                }
            }
        }
    }
    
    private void DrawUILine() => DrawUILine(Color.black);
    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }
}
#endif
