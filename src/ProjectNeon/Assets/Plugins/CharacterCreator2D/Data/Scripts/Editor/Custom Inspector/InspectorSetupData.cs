using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;
using System.IO;

namespace CharacterEditor2D
{
    //[CustomEditor(typeof(SetupData))]
    //public class InspectorSetupData : Editor
    //{
    //    private SetupData _setup;

    //    void OnEnable()
    //    {
    //        _setup = (SetupData)target;
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //        if (GUILayout.Button("refresh"))
    //            refresh();
    //    }

    //    private void refresh()
    //    {
    //        if (_setup == null)
    //            return;

    //        _setup.partPack = new List<PartPack>();
    //        List<string> sourcepath = getSourcePath();
    //        foreach (string s in sourcepath)
    //        {
    //            string packagename = Path.GetFileNameWithoutExtension(s);
    //            Debug.Log(packagename);
    //            List<Part> parts = EditorUtils.GetScriptables<Part>(s, true);
    //            foreach (Part p in parts)
    //            {
    //                p.packageName = packagename;
    //                EditorUtility.SetDirty(p);

    //                PartPack tpack = getPack(p.category, _setup.partPack);
    //                if (tpack == null)
    //                {
    //                    tpack = new PartPack();
    //                    tpack.category = p.category;
    //                    _setup.partPack.Add(tpack);
    //                }
                    
    //                tpack.parts.Add(p);
    //            }
    //        }
    //        EditorUtility.SetDirty(_setup);
    //    }

    //    private List<string> getSourcePath()
    //    {
    //        List<string> val = new List<string>();
    //        string defpath = "Assets/2DCharacterCreator/Parts";
    //        string[] directories = Directory.GetDirectories(defpath);
    //        foreach (string d in directories)
    //            val.Add(d);

    //        return val;
    //    }

    //    private PartPack getPack(PartCategory category, List<PartPack> pack)
    //    {
    //        foreach (PartPack p in pack)
    //        {
    //            if (p.category == category)
    //                return p;
    //        }
    //        return null;
    //    }
    //}
}