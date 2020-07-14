using UnityEngine;

#if UNITY_EDITOR_WIN
using UnityEngine.Windows;
#endif

public class EzScreen : CrossSceneSingleInstance
{
    [SerializeField] private string filename;
    
    protected override string UniqueTag => "Screenshots";
    private static int _counter;

    protected override void OnAwake()
    {
#if UNITY_EDITOR_WIN
        while (File.Exists($"{filename}_{_counter}.png"))
            _counter++;
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            var n = $"{filename}_{_counter++}.png";
            ScreenCapture.CaptureScreenshot(n);
            Debug.Log($"Captured screenshot: {n}");
        }
    }
}
