using System.Linq;
using UnityEditor;

public class CopyOldEnemyDataIntoNewForm
{
    [MenuItem("Tools/Neon/Copy Enemy Data From Old Form To New Form")]
    public static void Go()
    { 
        AssetDatabase.FindAssets("t:" + typeof(Enemy).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<Enemy>(AssetDatabase.GUIDToAssetPath(x)))
            .ForEach(x =>
            {
                x.CopyDataToNewForm();
                EditorUtility.SetDirty(x);
            });
    }
}