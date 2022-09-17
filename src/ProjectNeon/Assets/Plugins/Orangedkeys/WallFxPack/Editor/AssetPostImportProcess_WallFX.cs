using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Orangedkeys.WallFX;
using System.IO;



public class AssetPostImportProcess_WallFX : AssetPostprocessor
{
    static private bool WelcomeWin = false;
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        WelcomeWin = false;
        foreach (string item in importedAssets)
        {
            if (Path.GetFileName(item) == "AssetPostImportProcess_WallFX.cs") WelcomeWin = true;
            Debug.Log(item);
        }


        foreach (string itemdel in deletedAssets)
        {
            if (Path.GetFileName(itemdel) == "AssetPostImportProcess_WallFX.cs") WelcomeWin = false;

        }

        if (WelcomeWin)
        {
            Debug.Log("WALL FX PACK IMPORTED !!");
            WallFX_Welcome.ShowWindow();
        }

       

    }

}