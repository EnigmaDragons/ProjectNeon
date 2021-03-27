using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;
using System.IO;

namespace CharacterEditor2D
{
    [CustomEditor(typeof(PartList))]
    public class InspectorPartList : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("refresh"))
                refresh();
        }

        private void refresh()
        {
            PartList partlist = (PartList)target;
            if (partlist == null)
                return;

            RefreshPartPackage();
            Refresh(partlist);
        }

        public static void Refresh(PartList partList)
        {
            if (partList == null)
                return;

            partList.partPacks = new List<PartPack>();
            List<string> sourcepath = GetSourcePath();
            foreach (string s in sourcepath)
            {
                List<Part> parts = EditorUtils.GetScriptables<Part>(s, true);
                foreach (Part p in parts)
                {
                    PartPack tpack = GetPack(p.category, partList.partPacks);
                    if (tpack == null)
                    {
                        tpack = new PartPack();
                        tpack.category = p.category;
                        partList.partPacks.Add(tpack);
                    }

                    tpack.parts.Add(p);
                }
            }
            EditorUtility.SetDirty(partList);
            PartRefCustomMenu.RefreshPartReferers();
		}

		public static void RefreshPartPackage()
		{
			List<string> sourcepath = GetSourcePath();
			foreach (string s in sourcepath)
			{
				string packagename = Path.GetFileNameWithoutExtension(s);
				List<Part> parts = EditorUtils.GetScriptables<Part>(s, true);
				foreach (Part p in parts)
				{
                    if (p.packageName != packagename)
					{
                        p.packageName = packagename;
					    EditorUtility.SetDirty(p);
                    }
				}
			}
		}

        public static List<string> GetSourcePath()
        {
            List<string> val = new List<string>();
            string[] directories = Directory.GetDirectories(WizardUtils.PartFolder);
            foreach (string d in directories)
                val.Add(d);

            return val;
        }

        public static PartPack GetPack(PartCategory category, List<PartPack> pack)
        {
            foreach (PartPack p in pack)
            {
                if (p.category == category)
                    return p;
            }
            return null;
        }
    }
}