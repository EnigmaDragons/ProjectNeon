
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEditor;

public class TMP_InputStyleHack
{

    // and the method info for the method which must be called to reinitialise the style sheet after it's updated
    static MethodInfo m_minfoTMPStyleSheet_LoadStyleDictionaryInternal = typeof(TMP_StyleSheet).GetMethod("LoadStyleDictionaryInternal", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

    // grab the field info for TMP_StyleSheet's internal style list
    static FieldInfo m_finfoTMPStyleSheet_m_StyleList = typeof(TMP_StyleSheet).GetField("m_StyleList", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

    // grab the internal strings for initialising a TMP_Style at runtime
    static FieldInfo m_finfoTMPStyle_m_OpeningDefinition = typeof(TMP_Style).GetField("m_OpeningDefinition", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));
    static FieldInfo m_finfoTMPStyle_m_ClosingDefinition = typeof(TMP_Style).GetField("m_ClosingDefinition", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));


    public struct StyleStruct
    {
        public string name;
        public string openingTag;
        public string closingTag;

        public StyleStruct(string name, string openingTag, string closingTag)
        {
            this.name = name;
            this.openingTag = openingTag;
            this.closingTag = closingTag;
        }
    }

    //----------------------------------
    public static void AddOrReplaceStyle(string styleName, string openingTag, string closingTag)
    {
        List<StyleStruct> styles = new List<StyleStruct>();
        styles.Add(new StyleStruct(styleName, openingTag, closingTag));
        
        CreateStyles(styles);
    }

    /// <summary>
    /// Creates empty entries in the default style sheet to be later filled with actual values.
    /// IMPORTANT: before filling entries with actual values, the style sheet needs to update. Do this by
    /// changing a field in the sprite sheet slightly.
    /// </summary>
    /// <param name="entries"></param>
    public static void PrepareCreateStyles(List<StyleStruct> entries)
    {

        List<TMP_Style> stylesList = (List<TMP_Style>) m_finfoTMPStyleSheet_m_StyleList.GetValue(TMP_Settings.GetStyleSheet());

        for (int i=0; i<entries.Count; i++)
        {
            stylesList.Add(null);
        }

        SelectDefaultStyleSheetInHierarchy();
    }

    public static void UpdateStyles(List<StyleStruct> entries)
    {
        List<TMP_Style> stylesList = (List<TMP_Style>) m_finfoTMPStyleSheet_m_StyleList.GetValue(TMP_Settings.GetStyleSheet());

        for (int j = 0; j < entries.Count; j++)
        {
            int foundInstances = 0;
            for (int i = 0; i < stylesList.Count; ++i)
            {
                if (stylesList[i] == null)
                    continue;

                if (stylesList[i].name == entries[j].name)
                {
                    stylesList[i].name = entries[j].name;
                    m_finfoTMPStyle_m_OpeningDefinition.SetValue(stylesList[i], entries[j].openingTag);
                    m_finfoTMPStyle_m_ClosingDefinition.SetValue(stylesList[i], entries[j].closingTag);

                    stylesList[i].RefreshStyle();
                    foundInstances++;
                }
            }
        }

        TMP_Settings.GetStyleSheet().RefreshStyles();
    }

    public static int CreateStyles(List<StyleStruct> entries)
    {

        List<TMP_Style> stylesList = (List<TMP_Style>) m_finfoTMPStyleSheet_m_StyleList.GetValue(TMP_Settings.GetStyleSheet());
        int c = 0;

        for (int j = 0; j<entries.Count; j++)
        {

            for (int i = 0; i < stylesList.Count; ++i)
            {

                if (stylesList[i].name == "" || stylesList[i].name == entries[j].name)
                {
                    stylesList[i].name = entries[j].name;
                    m_finfoTMPStyle_m_OpeningDefinition.SetValue(stylesList[i], entries[j].openingTag);
                    m_finfoTMPStyle_m_ClosingDefinition.SetValue(stylesList[i], entries[j].closingTag);

                    stylesList[i].RefreshStyle();
                    c++;
                    break;

                }
            }
        }

        TMP_Settings.GetStyleSheet().RefreshStyles();
        SelectDefaultStyleSheetInHierarchy();
#if UNITY_EDITOR
        EditorUtility.SetDirty(TMP_Settings.GetStyleSheet());
#endif
        return c;
    }

    public static void RemoveEmptyEntriesInStyleSheet()
    {
        List<TMP_Style> stylesList = (List<TMP_Style>) m_finfoTMPStyleSheet_m_StyleList.GetValue(TMP_Settings.GetStyleSheet());
        for (int i = 0; i < stylesList.Count; ++i)
        {

            if (stylesList[i].name == "")
            {
                stylesList.RemoveAt(i);
                i--;
            }
        }
    }

    public static void RemoveAllEntries()
    {
        // grab the field info for TMP_StyleSheet's internal style list
        FieldInfo m_finfoTMPStyleSheet_m_StyleList = typeof(TMP_StyleSheet).GetField("m_StyleList", (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic));

        List<string> styleData = InputIcons.InputIconsManagerSO.Instance.GetAllBindingNames();

        List<TMP_Style> lstStyleList = (List<TMP_Style>) m_finfoTMPStyleSheet_m_StyleList.GetValue(TMP_Settings.GetStyleSheet());

        // check for the style already in the stylesheet & remove it if it is
        for (int i = lstStyleList.Count - 1; i >= 0; i--)
        {
            if (styleData.Contains(lstStyleList[i].name))
                lstStyleList.RemoveAt(i);

        }

        TMP_Settings.GetStyleSheet().RefreshStyles();
        SelectDefaultStyleSheetInHierarchy();
    }

    private static void SelectDefaultStyleSheetInHierarchy()
    {
#if UNITY_EDITOR
        Selection.activeObject = TMP_Settings.defaultStyleSheet;
        EditorGUIUtility.PingObject(Selection.activeObject);
#endif
    }
}
