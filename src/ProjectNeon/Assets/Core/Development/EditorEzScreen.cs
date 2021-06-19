using UnityEngine;

#if UNITY_EDITOR_WIN
using UnityEngine.Windows;
#endif

public class EditorEzScreen : MonoBehaviour
{
    [SerializeField] private string filename;
    
    private static int _counter;

#if UNITY_EDITOR_WIN
    private void Awake()
    {
        while (File.Exists($"{filename}_{_counter}.png"))
            _counter++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            var n = $"{filename}_{_counter++}.png";
            ScreenCapture.CaptureScreenshot(n);
            Log.Info($"Captured screenshot: {n}");
        }
    }
#endif
}
