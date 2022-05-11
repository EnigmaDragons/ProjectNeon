using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Custom editor for the MMTilemapGenerator, handles generate button and reorderable layers
    /// </summary>
    [CustomEditor(typeof(MMTilemapGenerator), true)]
    [CanEditMultipleObjects]
    public class MMTilemapGeneratorEditor : Editor
    {
    
        protected ReorderableList _list;

        protected virtual void OnEnable()
        {
            _list = new ReorderableList(serializedObject.FindProperty("Layers"));
            _list.elementNameProperty = "Layer";
            _list.elementDisplayType = ReorderableList.ElementDisplayType.Expandable;
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawPropertiesExcluding(serializedObject,  "Layers");
            EditorGUILayout.Space(10);
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            
            if (GUILayout.Button("Generate"))
            {
                (target as MMTilemapGenerator).Generate();
            }
        }
    }
}
