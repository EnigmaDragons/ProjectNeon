using System;
using UnityEngine;

public class CameraRenderTextureCleaner : MonoBehaviour
{
    void OnDisable()
    {
        try
        {
            Camera.allCameras.ForEach(c => c.targetTexture = null);
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}
