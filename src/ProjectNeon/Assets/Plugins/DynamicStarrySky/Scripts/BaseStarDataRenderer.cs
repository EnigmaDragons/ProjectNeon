using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Funly.DSS {
  // Base class for data star rendering implementations.
  public abstract class BaseStarDataRenderer : System.Object {
    public delegate void StarDataProgress(BaseStarDataRenderer renderer, float progress);
    public delegate void StarDataComplete(BaseStarDataRenderer renderer, Texture2D texture, bool success);

    public event StarDataProgress progressCallback;
    public event StarDataComplete completionCallback;

    public float density;
    public float imageSize;
    public float starRadius;

    protected float sphereRadius = 1.0f;
    
    // This is a coroutine so we don't block the main editor thread.
    public abstract IEnumerator ComputeStarData();
    
    protected void SendProgress(float progress)
    {
      if (progressCallback != null) {
        progressCallback(this, progress);
      }
    }

    protected void SendCompletion(Texture2D texture, bool success)
    {
      if (completionCallback != null) {
        completionCallback(this, texture, success);
      }
    }
  }
}