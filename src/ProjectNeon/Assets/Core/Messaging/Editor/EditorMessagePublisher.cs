#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestJsonMessagePublisher))]
public class TestMessagePublisherEditorGui : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var s = (TestJsonMessagePublisher)target;
        if(GUILayout.Button("Publish")) 
            s.Publish();
    }
}
#endif
