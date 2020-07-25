using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private Card targetCard;
    private SerializedProperty art, description, typeDescription, onlyPlayableByClass, cost, actionSequences, cardAction1, cardAction2;

    public void OnEnable()
    {
        targetCard = (Card) target;
        art = serializedObject.FindProperty("art");
        description = serializedObject.FindProperty("description");
        typeDescription = serializedObject.FindProperty("typeDescription");
        onlyPlayableByClass = serializedObject.FindProperty("onlyPlayableByClass");
        cost = serializedObject.FindProperty("cost");
        actionSequences = serializedObject.FindProperty("actionSequences");
        cardAction1 = serializedObject.FindProperty("cardAction1");
        cardAction2 = serializedObject.FindProperty("cardAction2");
    }

    public override void OnInspectorGUI()
    {
        PresentUnchanged(art);
        PresentUnchanged(description);
        PresentUnchanged(typeDescription);
        PresentUnchanged(onlyPlayableByClass);
        PresentUnchanged(cost);
        PresentActionSequences();
        PresentUnchanged(actionSequences);
        PresentUnchanged(cardAction1);
        PresentUnchanged(cardAction2);
    }

    private void PresentUnchanged(SerializedProperty serializedProperty)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedProperty);
        serializedObject.ApplyModifiedProperties();
    }

    private void PresentActionSequences()
    {
        EditorGUILayout.PrefixLabel("Action Sequences");
        var clickArea =  GUILayoutUtility.GetLastRect();
        Event current = Event.current;
        if (clickArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Insert New"), false, () =>
            {
                targetCard.actionSequences = new CardActionSequence[] {new CardActionSequence()}.Concat(targetCard.actionSequences).ToArray();
                EditorUtility.SetDirty(target);
            });
            menu.ShowAsContext();
            current.Use(); 
        }

    }
}