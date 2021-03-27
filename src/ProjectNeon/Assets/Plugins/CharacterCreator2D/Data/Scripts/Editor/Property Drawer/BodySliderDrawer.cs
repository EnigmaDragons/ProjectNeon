using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CharacterCreator2D
{
    [CustomPropertyDrawer(typeof(BodySliderAttribute))]
    public class BodySliderDrawer : PropertyDrawer
    {
        private static float _height = 16.0f;
        private static float _tab = 15.0f;
        private static float _vlabelwidth = 47;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
                return _height * 3.0f;
            else
                return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            BodySliderAttribute attr = attribute as BodySliderAttribute;
            Vector2 val = property.vector2Value;

            Rect labelrect = new Rect(position.x, position.y, position.width, _height);
            Rect xlabelrect = new Rect(position.x + _tab, labelrect.yMax, _vlabelwidth, _height);
            Rect xrect = new Rect(xlabelrect.xMax, xlabelrect.y, position.xMax - xlabelrect.xMax, _height);
            Rect ylabelrect = new Rect(position.x + _tab, xlabelrect.yMax, _vlabelwidth, _height);
            Rect yrect = new Rect(ylabelrect.xMax, ylabelrect.y, position.xMax - ylabelrect.xMax, _height);

            EditorGUI.LabelField(labelrect, label.text);
            EditorGUI.LabelField(xlabelrect, "X");
            val.x = EditorGUI.Slider(xrect, val.x, attr.minVal, attr.maxVal);
            EditorGUI.LabelField(ylabelrect, "Y");
            val.y = EditorGUI.Slider(yrect, val.y, attr.minVal, attr.maxVal);

            if (attr.symmetrical)
            {
                if (val.x != property.vector2Value.x && val.y == property.vector2Value.y)
                    val.y = val.x;
                else if (val.x == property.vector2Value.x && val.y != property.vector2Value.y)
                    val.x = val.y;
            }

            property.vector2Value = val;
        }
    }
}