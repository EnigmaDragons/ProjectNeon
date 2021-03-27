using System.Collections.Generic;
using System.Linq;
using CharacterCreator2D;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CharacterEditor2D
{
    [InitializeOnLoad]
    public static class PartRefCustomMenu
    {
        private class InternalProcessor : IActiveBuildTargetChanged, IPreprocessBuildWithReport
        {
            int IOrderedCallback.callbackOrder => 0;

            void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
            {
                EditorApplication.update += UpdatePartRefererUsageCheckbox;
            }

            void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
            {
#if CC2D_RES
                RefreshPartReferers();
#endif
            }
        }

        private const string MENU_PARTREFERER = "Window/Character Creator 2D/Memory Optimization/";
        private const string MENU_PARTREFERER_ENABLE = MENU_PARTREFERER + "Enable";
        private const string MENU_PARTREFERER_REFRESH = MENU_PARTREFERER + "Refresh";
        private const string MENU_PARTREFERER_CLEANUP = MENU_PARTREFERER + "Cleanup";
        private const string SEARCH_PART_FILTER = "t:" + nameof(Part);
        private const string SEARCH_PARTREFERER_FILTER = "t:" + nameof(PartReferer);
        private const string CC2D_RES_FOLDER = "PartReferers";
        private const string CC2D_RES_PATH = "Assets/CharacterCreator2D/Data/Resources/" + CC2D_RES_FOLDER;
        private const string CC2D_RES_SYMBOL = "CC2D_RES";
        private const string CC2D_RES_KEY = "IsUsePartRes";

        private static readonly string[] m_searchPathPath = new string[] { WizardUtils.PartFolder };
        private static readonly string[] m_searchPartRefererPath = new string[] { CC2D_RES_PATH };

        static PartRefCustomMenu()
        {
            EditorApplication.update += UpdatePartRefererUsageCheckbox;
        }

        private static void UpdatePartRefererUsageCheckbox()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            Menu.SetChecked(MENU_PARTREFERER_ENABLE, !string.IsNullOrEmpty(definesString) && definesString.Contains(CC2D_RES_SYMBOL));
            EditorApplication.update -= UpdatePartRefererUsageCheckbox;
        }

        [DidReloadScripts]
        private static void CheckPartRefererUsage()
        {
            EditorApplication.update += UpdatePartRefererUsageCheckbox;
            if (EditorPrefs.HasKey(CC2D_RES_KEY))
            {
                bool m_isUsageEnabled = EditorPrefs.GetBool(CC2D_RES_KEY, false);
                if (m_isUsageEnabled)
                {
                    if (EditorUtility.DisplayDialog("Refresh Part Referer Confirmation", "Would You like to refresh Part Referers?", "Yes", "No"))
                    {
                        RefreshPartReferers();
                    }
                }
                else
                {
                    if (AssetDatabase.IsValidFolder(CC2D_RES_PATH))
                    {
                        if (EditorUtility.DisplayDialog("Cleanup Part Referer Confirmation", "Would You like to cleanup Part Referers?", "Yes", "No"))
                        {
                            CleanupPartReferers();
                        }
                    }
                }
                EditorPrefs.DeleteKey(CC2D_RES_KEY);
            }
        }

        [MenuItem(MENU_PARTREFERER_ENABLE, false, 220)]
        private static void CheckAddressableUsage()
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            if (allDefines.Contains(CC2D_RES_SYMBOL))
            {
                allDefines.RemoveAll(s => s.Equals(CC2D_RES_SYMBOL));
                Menu.SetChecked(MENU_PARTREFERER_ENABLE, false);
                EditorPrefs.SetBool(CC2D_RES_KEY, false);
            }
            else
            {
                allDefines.Add(CC2D_RES_SYMBOL);
                Menu.SetChecked(MENU_PARTREFERER_ENABLE, true);
                EditorPrefs.SetBool(CC2D_RES_KEY, true);
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
        }

        [MenuItem(MENU_PARTREFERER_REFRESH, true, 240)]
        private static bool RefreshPartReferersValidator()
        {
#if CC2D_RES
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return false;
            }
            return true;
#else
            return false;
#endif
        }

        [MenuItem(MENU_PARTREFERER_REFRESH, false, 240)]
        public static void RefreshPartReferers()
        {
#if CC2D_RES
            AssetDatabase.ForceReserializeAssets(m_searchPathPath);
            AssetDatabase.Refresh();
            Dictionary<string, PartReferer> lastPartRefererDict = GetCurrentPartRefererDict();
            int counter = 0;
            var partPacks = PartList.Static.partPacks;
            foreach (PartPack partPack in partPacks)
            {
                int counter2 = 0;
                foreach (Part partObject in partPack.parts)
                {
                    string partRefPath = InitPartReferer(partObject, lastPartRefererDict);
                    EditorUtility.DisplayProgressBar("Initialize Part Referers", WizardUtils.RelativePartPath(partObject),
                        (float)counter / partPacks.Count + (float)counter2 / partPack.parts.Count / partPacks.Count);
                    counter2++;
                }
                counter++;
            }
            counter = 0;
            foreach (var lastPartReferer in lastPartRefererDict)
            {
                AssetDatabase.DeleteAsset(lastPartReferer.Key);
                EditorUtility.DisplayCancelableProgressBar("Delete Unused Part Referers", lastPartReferer.Value.name,
                        (float)counter / lastPartRefererDict.Count);
                counter++;
            }
            AssetDatabase.Refresh();
            EditorUtility.UnloadUnusedAssetsImmediate(true);
            EditorUtility.ClearProgressBar();
#endif
        }

        [MenuItem(MENU_PARTREFERER_CLEANUP, true, 241)]
        private static bool CleanupPartReferersValidator()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return false;
            }
            return true;
        }

        [MenuItem(MENU_PARTREFERER_CLEANUP, false, 241)]
        public static void CleanupPartReferers()
        {
            if (AssetDatabase.IsValidFolder(CC2D_RES_PATH))
            {
                AssetDatabase.DeleteAsset(CC2D_RES_PATH);
                AssetDatabase.Refresh();
            }
        }

#if CC2D_RES

        private static string InitPartReferer(Part partObject, Dictionary<string, PartReferer> lastPartRefererDict)
        {
            string relativePartRefPath = WizardUtils.RelativePartPath(partObject);
            string partRefPath = CC2D_RES_PATH + "/" + relativePartRefPath;
            if (EditorUtils.MakeSureAssetFolderExist(partRefPath.Remove(partRefPath.LastIndexOf("/"))))
            {
                if (lastPartRefererDict.TryGetValue(partRefPath, out PartReferer result))
                {
                    AssignPartReferer(partObject, result);
                    lastPartRefererDict.Remove(partRefPath);
                }
                else
                {
                    PartReferer newPartReferer = ScriptableObject.CreateInstance<PartReferer>();
                    AssignPartReferer(partObject, newPartReferer);
                    AssetDatabase.CreateAsset(newPartReferer, partRefPath);
                }
                partObject.datapath = CC2D_RES_FOLDER + "/" + relativePartRefPath.Remove(relativePartRefPath.LastIndexOf(".asset"));
            }
            return partRefPath;
        }

        private static void AssignPartReferer(Part partObject, PartReferer result)
        {
            result.texture = partObject.texture;
            result.colorMask = partObject.colorMask;
            if (partObject.sprites != null)
            {
                result.sprites = new List<Sprite>(partObject.sprites);
            }
            else
            {
                result.sprites = new List<Sprite>();
            }
        }

        private static Dictionary<string, PartReferer> GetCurrentPartRefererDict()
        {
            Dictionary<string, PartReferer> lastPartRefererDict = new Dictionary<string, PartReferer>();
            int counter = 0;
            if (EditorUtils.MakeSureAssetFolderExist(CC2D_RES_PATH))
            {
                string[] foundPartReferers = AssetDatabase.FindAssets(SEARCH_PARTREFERER_FILTER, m_searchPartRefererPath);
                foreach (var partRefGuid in foundPartReferers)
                {
                    string partRefPath = AssetDatabase.GUIDToAssetPath(partRefGuid);
                    PartReferer partRefObject = AssetDatabase.LoadAssetAtPath<PartReferer>(partRefPath);
                    lastPartRefererDict.Add(partRefPath, partRefObject);
                    EditorUtility.DisplayProgressBar("Load Current Part Referers", partRefObject.name, (float)counter / foundPartReferers.Length);
                    counter++;
                }
                EditorUtility.ClearProgressBar();
            }
            return lastPartRefererDict;
        }

#endif
    }
}
