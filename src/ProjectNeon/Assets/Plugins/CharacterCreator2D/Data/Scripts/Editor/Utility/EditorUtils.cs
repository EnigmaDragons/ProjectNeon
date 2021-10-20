using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CharacterEditor2D
{
    public static class EditorUtils
    {
        public static string GetAssetPath(string completePath)
        {
            string val = "";

            if (completePath.Contains(Application.dataPath)) //..jika path contains project path
            {
                int assetindex = completePath.IndexOf("Assets/");
                val = completePath.Substring(assetindex);
            }

            return val;
        }

        public static string RelativeToFullPath(string relativePath)
        {
            if (relativePath is null)
            {
                relativePath = "";
            }
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativePath)).Replace('\\', '/');
        }

        public static string CleanupAssetPath(string assetPath)
        {
            assetPath = assetPath.Replace("\\", "/");
            assetPath = assetPath.Replace("//", "/");
            int lastSlash = assetPath.LastIndexOf('/');
            if (lastSlash == assetPath.Length - 1)
            {
                assetPath = assetPath.Remove(lastSlash);
            }
            return assetPath;
        }

        public static bool MakeSureAssetFolderExist(string assetPath)
        {
            assetPath = CleanupAssetPath(assetPath);
            return MakeSureAssetFolderExistInternal(assetPath);
        }

        private static bool MakeSureAssetFolderExistInternal(string assetPath)
        {
            AssetDatabase.Refresh();
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                // folder is exist already
                return true;
            }
            int lastSparatorIndex = assetPath.LastIndexOf('/');
            if (lastSparatorIndex < 0)
            {
                // it's not in asset folder
                return false;
            }
            string parentPath = assetPath.Remove(lastSparatorIndex);
            if (!MakeSureAssetFolderExistInternal(parentPath))
            {
                // it's not in asset folder
                return false;
            }
            string folderName = assetPath.Substring(lastSparatorIndex + 1);
            if (string.IsNullOrEmpty(folderName))
            {
                // folder end with "/", so still valid
                return true;
            }
            string result = AssetDatabase.CreateFolder(parentPath, folderName);
            if (!string.IsNullOrEmpty(result))
            {
                // success with result of guid folder
                return true;
            }
            // cannot create asset fodler
            return false;
        }

        public static T LoadScriptable<T>(string path) where T : UnityEngine.ScriptableObject
        {
            T val = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));

            if (val == null)
            {
                val = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(val, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return val;
        }

        public static List<T> GetPrefabs<T>(string path) where T : UnityEngine.MonoBehaviour
        {
            return GetPrefabs<T>(path, false);
        }

        public static List<T> GetPrefabs<T>(string path, bool readThroughFolders) where T : UnityEngine.MonoBehaviour
        {
            List<T> val = new List<T>();

            string[] files = readThroughFolders ? Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories) :
                Directory.GetFiles(path, "*.prefab");

            foreach (string f in files)
            {
                T temp = (T)AssetDatabase.LoadAssetAtPath(f, typeof(T));
                if (temp != null)
                {
                    val.Add(temp);
                }
            }

            return val;
        }

        public static List<T> GetScriptables<T>(string path) where T : UnityEngine.ScriptableObject
        {
            return GetScriptables<T>(path, false);
        }

        public static List<T> GetScriptables<T>(string path, bool readThroughFolders) where T : UnityEngine.ScriptableObject
        {
            if (!Directory.Exists(path))
            {
                return new List<T>();
            }

            List<T> val = new List<T>();
            string[] files = readThroughFolders ? Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories) :
                Directory.GetFiles(path, "*.asset");
            foreach (string f in files)
            {
                T temp = (T)AssetDatabase.LoadAssetAtPath(f, typeof(T));
                if (temp != null)
                {
                    val.Add(temp);
                }
            }
            return val;
        }

        public static List<T> GetSubAssets<T>(string assetPath, bool includeHidden = true) where T : Object
        {
            List<T> result = new List<T>();
            if (File.Exists(RelativeToFullPath(assetPath)))
            {
                Object[] assets;
                if (includeHidden)
                {
                    assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                }
                else
                {
                    assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                }
                foreach (Object item in assets)
                {
                    if (AssetDatabase.IsSubAsset(item) && item is T itemT)
                    {
                        result.Add(itemT);
                    }
                }
            }
            return result;
        }

        public static Texture2D CreateTexture(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
