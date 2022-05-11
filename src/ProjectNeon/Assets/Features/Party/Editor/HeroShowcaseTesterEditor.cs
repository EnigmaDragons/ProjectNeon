#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeroShowcaseTester))]
public class HeroShowcaseTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var tester = (HeroShowcaseTester)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Begin Showcase"))
            tester.BeginShowcase();
    }
}
#endif
