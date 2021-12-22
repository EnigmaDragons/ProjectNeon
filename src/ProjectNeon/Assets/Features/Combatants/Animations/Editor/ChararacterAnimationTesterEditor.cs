using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterAnimationTester))]
public class ChararacterAnimationTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var tester = (CharacterAnimationTester) target;
        DrawDefaultInspector();

        if (GUILayout.Button("Play Animation"))
            tester.Play();
    }
}