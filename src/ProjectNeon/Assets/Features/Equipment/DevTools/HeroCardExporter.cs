using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class HeroCardExporter : MonoBehaviour
{
    [SerializeField] private string baseExportPathDir;
    [SerializeField] private AllHeroes heroes;
    [SerializeField] private HeroCardExportPresenter presenter;
    [SerializeField] private Color transparentColor = Color.black;

    private void Start() => this.SafeCoroutineOrNothing(Go());

    private IEnumerator Go()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        presenter.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        foreach (var x in heroes.Heroes.Where(h => h.BodySprite != null))
        {
            var err = false;
            try
            {
                presenter.Init(x);
            }
            catch (Exception e)
            {
                err = true;
                Log.Warn($"Unable to Render {x.NameTerm().ToEnglish()}");
                Log.Error(e);
            }

            if (!err)
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                ExportScreenshot("HeroCard-" + x.NameTerm().ToEnglish());
            }
        }
        
        Log.Info("Finished Export");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    private void ExportScreenshot(string fileName)
    {
        var savePath = Path.Combine(baseExportPathDir, fileName + ".png");
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var newTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        var pixels = tex.GetPixels();
        for (var i = 0; i < pixels.Length; i++)
        {
            var p = pixels[i];
            if (p.r == transparentColor.r && p.g == transparentColor.g && p.b == transparentColor.b)
                p = Color.clear;
        }

        newTexture.SetPixels(pixels);
        var pngShot = ImageConversion.EncodeToPNG(newTexture);
        File.WriteAllBytes(savePath, pngShot);
    }
}
