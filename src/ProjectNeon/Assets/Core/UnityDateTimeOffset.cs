using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

[Serializable]
public class UnityDateTimeOffset : ISerializationCallbackReceiver
{
    [HideInInspector] [SerializeField] private string _dateTimeOffset;
    [HideInInspector] public DateTimeOffset dateTimeOffset;
    public static implicit operator DateTimeOffset(UnityDateTimeOffset udto) => udto.dateTimeOffset.ToUniversalTime();
    public static implicit operator UnityDateTimeOffset(DateTimeOffset dto) => new UnityDateTimeOffset() { dateTimeOffset = dto.ToUniversalTime() };
    public void OnAfterDeserialize() => DateTimeOffset.TryParse(_dateTimeOffset, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTimeOffset);
    public void OnBeforeSerialize() => _dateTimeOffset = dateTimeOffset.ToString("g");
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UnityDateTimeOffset))]
public class UnityDateTimeOffsetDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        Rect amountRect = new Rect(position.x, position.y, position.width, position.height);
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("_dateTimeOffset"), GUIContent.none);
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
#endif