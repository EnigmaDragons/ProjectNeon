using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;

namespace CharacterEditor2D
{
    [CustomEditor(typeof(CharacterViewer))]
    public class CharacterViewerEditor : Editor
    {
        private CharacterViewer _character;
        private SerializedProperty _initialized;
        private SerializedProperty _instancemat;
        private SerializedProperty _tintcolor;
        private bool _hidechild;

        private readonly GUIContent _initializedgui = new GUIContent()
        {
            text = "Initialize On Awake",
            tooltip = "Initialized the CharacterViewer at awake. You can turn this off to save performance if you don't have any plan to customize this character at runtime."
        };
        private readonly GUIContent _instancematgui = new GUIContent()
        {
            text = "Instantiate Material",
            tooltip = "Instantiate materials for each character when the application is playing. You can turn this off to save performance. Turning this off will make every instance of the prefab uses shared materials"
        };
        private readonly GUIContent _tintgui = new GUIContent()
        {
            text = "Tint Color",
            tooltip = "Character's tint color"
        };
        private readonly GUIContent _hidechildui = new GUIContent()
        {
            text = "Auto Hide Children",
            tooltip = "Automatically hide children in hierarchy (Shared settings)"
        };

        void OnEnable()
        {
            _character = (CharacterViewer)target;
            _initialized = serializedObject.FindProperty("initializeAtAwake");
            _instancemat = serializedObject.FindProperty("instanceMaterials");
            _tintcolor = serializedObject.FindProperty("_tintcolor");
            _hidechild = EditorPrefs.GetBool("Simpleton/CC2D/autohidechild",true);
            if(_hidechild)
            HideInHierarchy(_character);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Color tcolor = _character.TintColor;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", WizardUtils.BoldTextStyle);
            EditorGUILayout.PropertyField(_initialized, _initializedgui);
            EditorGUILayout.PropertyField(_instancemat, _instancematgui);
            EditorGUILayout.PropertyField(_tintcolor, _tintgui);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hierarchy", WizardUtils.BoldTextStyle);
            _hidechild = EditorGUILayout.Toggle(_hidechildui,_hidechild);
            EditorPrefs.SetBool("Simpleton/CC2D/autohidechild",_hidechild);

            if (GUILayout.Button("Show Child"))
                ShowInHierarchy(_character);
            if (GUILayout.Button("Hide Child"))
                HideInHierarchy(_character);
            serializedObject.ApplyModifiedProperties();

            if (tcolor != _tintcolor.colorValue)
                _character.RepaintTintColor();
        }

        void ShowInHierarchy(CharacterViewer character)
        {
            Transform[] child = character.GetComponentsInChildren<Transform>(true);
            foreach (Transform c in child)
            {
                c.gameObject.hideFlags = HideFlags.None;
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        void HideInHierarchy(CharacterViewer character)
        {
            Transform[] child = character.GetComponentsInChildren<Transform>(true);
            foreach (Transform c in child)
            {
                if (c != child[0])
                {
                    c.gameObject.hideFlags = HideFlags.HideInInspector;
                    c.gameObject.hideFlags ^= HideFlags.HideInHierarchy;
                }
            }
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.DirtyHierarchyWindowSorting();
        }
    }
}