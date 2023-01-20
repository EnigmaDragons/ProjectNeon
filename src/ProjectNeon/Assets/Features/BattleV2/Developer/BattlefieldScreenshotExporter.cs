using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class BattlefieldScreenshotExporter : MonoBehaviour
{
    [SerializeField] private string baseExportPathDir;
    [SerializeField] private BattlefieldSet[] sets;
    [SerializeField] private GameObject stage;
    [SerializeField] private bool takeAll = false;

    private void Start() => StartCoroutine(Go());

    private IEnumerator Go()
    {
        foreach (var s in sets)
        {
            foreach (var battlefield in s.Battlefields)
            {
                stage.DestroyAllChildrenImmediate();
                Instantiate(battlefield, stage.transform);
            
                for(var i = 0; i < 10; i++)
                    yield return new WaitForEndOfFrame();
                Export(battlefield.name);
                
                if (!takeAll)
                    yield break;
            }
        }
        Log.Info("Finished Export");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void Export(string fileName)
    {
        var savePath = Path.Combine(baseExportPathDir, fileName.Replace(" ", "").Replace("\"", "") + ".jpg");
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var newTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        var pixels = tex.GetPixels();

        newTexture.SetPixels(pixels);
        var pngShot = ImageConversion.EncodeToJPG(newTexture);
        File.WriteAllBytes(savePath, pngShot);
    }
}
