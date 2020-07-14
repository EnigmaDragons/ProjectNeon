 using UnityEngine;
 using System.IO;
 
 // Screen Recorder will save individual images of active scene in any resolution and of a specific image format
 // including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
 //
 // You can compile these images into a video using ffmpeg:
 // ffmpeg -i screen_3840x2160_%d.ppm -y test.avi
 
 public class ScreenRecorder : MonoBehaviour
 {
     // 4k = 3840 x 2160   1080p = 1920 x 1080
     public int captureWidth = 1920;
     public int captureHeight = 1080;
 
     // optional game object to hide during screenshots (usually your scene canvas hud)
     public GameObject hideGameObject; 
 
     // optimize for many screenshots will not destroy any objects so future screenshots will be fast
     public bool optimizeForManyScreenshots = true;
 
     public enum ScreenshotFormat { RAW, JPG, PNG, PPM };
     public ScreenshotFormat format = ScreenshotFormat.PPM;
 
     // optional target folder
     public string folder;

     public AudioClip screenshotSound;
     public UiSfxPlayer soundPlayer;

     public bool enableVideoCapture = false;
     
     // private vars for screenshot
     private Rect rect;
     private RenderTexture renderTexture;
     private Texture2D screenShot;
     private int counter = 0; // image #
 
     // commands
     private bool captureScreenshot = false;
     private bool captureVideo = false;
 
     private string uniqueFilename(int width, int height)
     {
         if (folder == null || folder.Length == 0)
         {
             folder = Application.persistentDataPath;
             if (Application.isEditor)
             {
                 var stringPath = folder + "/..";
                 folder = Path.GetFullPath(stringPath);
             }
             folder += "/screenshots";
 
             Directory.CreateDirectory(folder);
 
             var mask = $"screen_{width}x{height}*.{format.ToString().ToLower()}";
             counter = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly).Length;
         }
 
         var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", folder, width, height, counter, format.ToString().ToLower());
        ++counter;
         return filename;
     }
 
     public void CaptureScreenshot()
     {
         captureScreenshot = true;
     }
 
     void Update()
     {
         captureVideo = enableVideoCapture && Input.GetKey("v");
 
         if (captureScreenshot || captureVideo)
         {
             if (soundPlayer != null && screenshotSound != null)
                 soundPlayer.Play(screenshotSound);
                     
             captureScreenshot = false;
 
             if (hideGameObject != null) 
                 hideGameObject.SetActive(false);
 
             if (renderTexture == null)
             {
                 rect = new Rect(0, 0, captureWidth, captureHeight);
                 renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
                 screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
             }
         
             var camera = Camera.main;
             camera.targetTexture = renderTexture;
             camera.Render();
             RenderTexture.active = renderTexture;
             screenShot.ReadPixels(rect, 0, 0);
                camera.targetTexture = null;
             RenderTexture.active = null;
 
             var filename = uniqueFilename((int) rect.width, (int) rect.height);
 
             // pull in our file header/data bytes for the specified image format (has to be done from main thread)
             byte[] fileHeader = null;
             byte[] fileData = null;
             if (format == ScreenshotFormat.RAW)
                 fileData = screenShot.GetRawTextureData();
             else if (format == ScreenshotFormat.PNG)
                 fileData = screenShot.EncodeToPNG();
             else if (format == ScreenshotFormat.JPG)
                 fileData = screenShot.EncodeToJPG();
             else 
             {
                 var headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
                 fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
                 fileData = screenShot.GetRawTextureData();
             }
 
             // create new thread to save the image to file (only operation that can be done in background)
             new System.Threading.Thread(() =>
             {
                 // create file and write optional header with image bytes
                 var f = File.Create(filename);
                 if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
                 f.Write(fileData, 0, fileData.Length);
                 f.Close();
                 Debug.Log($"Screenshot {filename}");
             }).Start();
 
             // unhide optional game object if set
             if (hideGameObject != null) hideGameObject.SetActive(true);
 
             // cleanup if needed
             if (optimizeForManyScreenshots == false)
             {
                 Destroy(renderTexture);
                 renderTexture = null;
                 screenShot = null;
             }
         }
     }
 }
