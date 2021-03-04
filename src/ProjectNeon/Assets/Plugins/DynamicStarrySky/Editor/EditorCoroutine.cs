using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace  Funly.DSS
{
  public class EditorCoroutine
  {
    public static EditorCoroutine start(IEnumerator _routine)
    {
      EditorCoroutine coroutine = new EditorCoroutine(_routine);
      coroutine.start();
      return coroutine;
    }

    readonly IEnumerator routine;
    EditorCoroutine(IEnumerator _routine)
    {
      routine = _routine;
    }

    void start()
    {
      EditorApplication.update += update;
    }
    public void stop()
    {
      EditorApplication.update -= update;
    }

    void update()
    {
      Debug.Log("update");
      if (!routine.MoveNext()) {
        stop();
      }
    }
  }
}

