using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using CharacterCreator2D;

namespace CharacterEditor2D
{    
    public static class CustomMenu
    {
        [MenuItem("Window/Character Creator 2D/Add New Part", false, 100)]
        public static void CreatePart()
        {
            ScriptableWizard.DisplayWizard<CharacterEditor2D.WizardPart>("Add Part", "Create");
        }

        [MenuItem("Window/Character Creator 2D/Refresh Parts and Add-Ons", false, 15)]
        public static void RefreshPartList()
        {
            if (PartList.Static != null)
            {
                InspectorPartList.RefreshPartPackage();
                InspectorPartList.Refresh(PartList.Static);
                EditorUtility.DisplayDialog("Refresh Part", "Parts and Add-Ons refreshed succesfully!", "Ok");
            }
        }
        
        [MenuItem("Window/Character Creator 2D/Documentation", false, 300)]
        public static void OpenDocumentation () 
        {
            UnityEngine.Application.OpenURL("http://bit.ly/CC2Ddoc");
        }
    }
    
    [InitializeOnLoad]
    public class PlayCreatorScene
    {
        static PlayCreatorScene()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static string startScene = "Assets/CharacterCreator2D/Creator UI/Creator UI.unity";
        static string prevScene = EditorPrefs.GetString("PlayFromStartPrevScene");
        static bool active = EditorPrefs.GetBool("PlayFromStartActive", false);

        [MenuItem("Window/Character Creator 2D/Create Character", false, 10)]
        static void Play()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                if (PartList.Static != null)
                {
                    InspectorPartList.RefreshPartPackage();
                    InspectorPartList.Refresh(PartList.Static);
                }
                EditorPrefs.SetString("PlayFromStartPrevScene", EditorSceneManager.GetActiveScene().path);
                EditorSceneManager.OpenScene(startScene);
                EditorPrefs.SetBool("PlayFromStartActive", true);
                EditorApplication.isPlaying = true;
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            if (!active) return;
            if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += RestoreScene;
            }
        }

        public static void RestoreScene()
        {
            if (EditorApplication.isPlaying) return;
            if (prevScene == null || prevScene == "")
            {
                EditorApplication.update -= RestoreScene;
                return;
            }
            EditorSceneManager.OpenScene(prevScene);
            EditorPrefs.DeleteKey("PlayFromStartPrevScene");
            EditorPrefs.DeleteKey("PlayFromStartActive");
            EditorApplication.update -= RestoreScene;
        }
    }
}