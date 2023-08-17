using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class BattlefieldScreenshotExporter : MonoBehaviour
{
    [SerializeField] private string baseExportPathDir;
    [SerializeField] private BattlefieldSet[] sets;
    [SerializeField] private GameObject stage;
    [SerializeField] private int framesToWaitBeforeTextureCapture = 500;
    [SerializeField] private bool takeAll = false;
    [SerializeField] private bool isForBakingAndNotScreenshots = false;
    [SerializeField] private GameObject bakeCamera;
    [SerializeField] private GameObject standardCamera;
    [SerializeField] private string nonBakePrefix = "MP0_";

    private void Start() => this.SafeCoroutineOrNothing(Go());

    private const string DoNotBakeTag = "DoNotBake";
    
    private IEnumerator Go()
    {
        bakeCamera.SetActive(isForBakingAndNotScreenshots);
        standardCamera.SetActive(!isForBakingAndNotScreenshots);
        var lowQualityMode = CurrentLowQualityMode.IsEnabled;
        CurrentLowQualityMode.Disable();
        foreach (var s in sets)
        {
            foreach (var battlefield in s.Battlefields)
            {
                stage.DestroyAllChildrenImmediate();
                var obj = Instantiate(battlefield, stage.transform);
                for(var i = 0; i < 10; i++)
                    yield return new WaitForEndOfFrame();
                if (isForBakingAndNotScreenshots)
                    obj.GetComponentsInChildren<Transform>(true)
                        .Where(t => t.CompareTag(DoNotBakeTag))
                        .ForEach(t => t.gameObject.SetActive(false));

                for(var i = 0; i < framesToWaitBeforeTextureCapture; i++)
                    yield return new WaitForEndOfFrame();
                Export(battlefield.name);
                
                if (!takeAll)
                    yield break;
            }
        }
        CurrentLowQualityMode.Set(lowQualityMode);
        Log.Info("Finished Export");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void Export(string fileName)
    {
        var withPrefix = isForBakingAndNotScreenshots ? fileName : nonBakePrefix + fileName;
        var savePath = Path.Combine(baseExportPathDir, withPrefix.Replace(" ", "").Replace("\"", "") + ".jpg");
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var newTexture = new Texture2D(tex.width, tex.height, TextureFormat.RGBA64, false);
        var pixels = tex.GetPixels();

        newTexture.SetPixels(pixels);
        var pngShot = ImageConversion.EncodeToJPG(newTexture, 95);
        File.WriteAllBytes(savePath, pngShot);
    }
}
