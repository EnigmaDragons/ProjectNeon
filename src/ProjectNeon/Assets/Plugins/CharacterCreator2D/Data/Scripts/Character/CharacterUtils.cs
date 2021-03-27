using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CharacterCreator2D
{
    public static class CharacterUtils
    {
        private static string _deffolder = Application.dataPath;

        /// <summary>
        /// [EDITOR] Display save file dialog.
        /// </summary>
        /// <param name="title">Title to display.</param>
        /// <param name="defaultName">The default name of the file.</param>
        /// <param name="extension">The extension of the file.</param>
        /// <param name="projectFolderOnly">Limit path within Unity project folder.</param>
        /// <returns>Selected path if there is, otherwise returns 'empty'</returns>
		public static string ShowSaveFileDialog(string title, string defaultName, string extension, bool projectFolderOnly)
        {
            string val = "";
#if UNITY_EDITOR
            val = EditorUtility.SaveFilePanel(title, _deffolder, defaultName, extension);
            if (string.IsNullOrEmpty(val))
                return "";
            val = getAssetPath(val, projectFolderOnly);
#endif
            return val;
        }

        /// <summary>
        /// [EDITOR] Display open file dialog.
        /// </summary>
        /// <param name="title">Title to display.</param>
        /// <param name="extension">The extension of the file.</param>
        /// <param name="projectFolderOnly">Limit path within Unity project folder.</param>
        /// <returns>Selected path if there is any, otherwise returns 'empty'</returns>
		public static string ShowOpenFileDialog(string title, string extension, bool projectFolderOnly)
        {
            string val = "";
#if UNITY_EDITOR
            val = EditorUtility.OpenFilePanel(title, _deffolder, extension);
            if (string.IsNullOrEmpty(val))
                return "";
            val = getAssetPath(val, projectFolderOnly);
#endif
            return val;
        }

        /// <summary>
        /// [Editor] Instantiate a CharacterViewer based on another CharacterViewer.
        /// </summary>
        /// <param name="baseCharacter">CharacterViewer to be copied from.</param>
        /// <param name="prefabPath">Path to the CharacterViewer prefab in Assets directory.</param>
        /// <returns>Instantiated CharacterViewer with the same settings as baseCharacter that linked to prefab in prefabPath. Returns an instantiated object of baseCharacter if there is no CharacterViewer found in prefabPath.</returns>
        public static CharacterViewer SpawnCharacter(CharacterViewer baseCharacter, string prefabPath)
        {
            CharacterViewer val = null;
#if UNITY_EDITOR
            CharacterViewer sourcecharacter = LoadCharacterFromPrefab(prefabPath);
            if (sourcecharacter == null)
            {
                val = GameObject.Instantiate<CharacterViewer>(baseCharacter);
                val.name = Path.GetFileNameWithoutExtension(prefabPath);

            }
            else
            {
                val = GameObject.Instantiate<CharacterViewer>(sourcecharacter);
                val.name = sourcecharacter.name;
                val.AssignCharacterData(baseCharacter.GenerateCharacterData());
            }
#endif
            return val;
        }

        /// <summary>
        /// [EDITOR] Save a CharacterViewer as a prefab in a selected path and automatically generate its materials.
        /// </summary>
        /// <param name="character">CharacterViewer to be saved as prefab.</param>
        /// <param name="path">The path must be in Assets directory.</param>
        /// <returns>'true' on succes, otherwise returns 'false'.</returns>
        public static bool SaveCharacterToPrefab(CharacterViewer sourceCharacter, string path)
        {
#if UNITY_EDITOR
            try
            {
                List<Material> materials = new List<Material>();

                //..get all character's materials
                foreach (SlotCategory c in Enum.GetValues(typeof(SlotCategory)))
                {
                    PartSlot slot = sourceCharacter.slots.GetSlot(c);
                    if (slot == null || slot.material == null || materials.Contains(slot.material))
                        continue;
                    slot.material.name = slot.material.name.Replace("CC2D", sourceCharacter.name);
                    materials.Add(slot.material);
                }
                //get all character's materials..

                string materialfolder = string.Format("{0}/{1}_materials", Path.GetDirectoryName(path), sourceCharacter.name);
                if (!Directory.Exists(materialfolder))
                {
                    Directory.CreateDirectory(materialfolder);
                    AssetDatabase.Refresh();
                }

                foreach (Material m in materials)
                {
                    string filename = string.Format("{0}/{1}.mat", materialfolder, m.name);
                    AssetDatabase.CreateAsset(m, filename);
                }
                AssetDatabase.Refresh();
                sourceCharacter.RelinkMaterials();

#if UNITY_2018_3_OR_NEWER
                if (AssetDatabase.LoadAssetAtPath<CharacterViewer>(path) != null)
                {
                    GameObject targetgo = PrefabUtility.LoadPrefabContents(path);
                    CharacterViewer targetchara = targetgo.GetComponent<CharacterViewer>();
                    Transform sourceroot = sourceCharacter.transform.Find("Root");
                    Transform targetroot = targetchara.transform.Find("Root");
                    if (targetroot != null)
                        GameObject.DestroyImmediate(targetroot.gameObject);
                    if (sourceroot != null)
                    {
                        targetroot = GameObject.Instantiate<Transform>(sourceroot);
                        targetroot.name = "Root";
                        targetroot.SetParent(targetchara.transform);
                        targetroot.localPosition = Vector3.zero;
                        EditorUtility.CopySerialized(sourceCharacter, targetchara);
                        PrefabUtility.SaveAsPrefabAsset(targetgo, path);
                        PrefabUtility.UnloadPrefabContents(targetgo);
                    }
                }
                else
                    PrefabUtility.SaveAsPrefabAsset(sourceCharacter.gameObject, path);
#else
                CharacterViewer extcharacter = AssetDatabase.LoadAssetAtPath<CharacterViewer>(path);
                if (extcharacter != null)
                    PrefabUtility.ReplacePrefab(sourceCharacter.gameObject, extcharacter);
                else
                    PrefabUtility.CreatePrefab(path, sourceCharacter.gameObject, ReplacePrefabOptions.ConnectToPrefab);
#endif

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("error on save character to prefab:\n" + e.ToString());
                return false;
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// [EDITOR] Load CharacterViewer from a given path.
        /// </summary>
        /// <param name="path">The path must be in Assets directory.</param>
        /// <returns>Character object if it's existed, otherwise returns 'null'.</returns>
        public static CharacterViewer LoadCharacterFromPrefab(string path)
        {
            CharacterViewer val = null;
#if UNITY_EDITOR
            try
            {
                val = AssetDatabase.LoadAssetAtPath<CharacterViewer>(path);

                //version exception..
                if (val != null && val.GetAssignedPart(SlotCategory.BodySkin) == null)
                {
                    Part missingskin = getMissingBodySkin(val);
                    if (missingskin != null)
                        val.EquipPart(SlotCategory.BodySkin, missingskin);
                }
                //..version exception
            }
            catch (Exception e)
            {
                Debug.LogError("error on load from prefab:\n" + e.ToString());
            }
#endif
            return val;
        }

            private static Part getMissingBodySkin(CharacterViewer character)
        {
            Dictionary<string, string> links = SetupData.partLinks[SlotCategory.BodySkin];
            List<Part> bodyskins = PartList.Static.FindParts(SlotCategory.BodySkin);
            foreach (Part skin in bodyskins)
            {
                bool found = false;
                foreach (Sprite sprite in skin.sprites)
                {
                    try
                    {
                        found = character.transform.Find(links[sprite.name]).GetComponent<SpriteRenderer>().sprite == sprite;
                        if (!found)
                            break;
                    }
                    catch
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return skin;
            }

            return null;
        }

        private static string getAssetPath(string completePath, bool projectFolderOnly)
        {
            string val = "";
            if (!projectFolderOnly)
            {
                val = completePath;
            }

#if UNITY_EDITOR
            if (completePath.Contains(Application.dataPath)) //..if the path contains project path
            {
                val = completePath.Replace(Application.dataPath, "Assets");
            }
            else if (projectFolderOnly)
            {
                Debug.LogError("CC2D - Invalid path: Path is outside of this Unity Project folder.");
            }
#endif

            return val;
        }
    }
}