using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CharacterCreator2D
{
    [CustomPropertyDrawer(typeof(ColorCodeAttribute))]
    public class ColorCodeDrawer : PropertyDrawer
    {
        private int[] _typeint = new int[] { 0, 1, 2 };
        private string[] _typestring = new string[]
            {
                ColorCode.Color1,
                ColorCode.Color2,
                ColorCode.Color3
            };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.stringValue == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            int tid = getSelectedIndex(property.stringValue);
            tid = EditorGUI.IntPopup(position, label.text, tid, _typestring, _typeint);
            property.stringValue = _typestring[tid];
        }

        private int getSelectedIndex(string colorCode)
        {
            for (int i = 0; i < _typestring.Length; i++)
            {
                if (colorCode == _typestring[i])
                    return _typeint[i];
            }
            return 0;
        }
    }
}