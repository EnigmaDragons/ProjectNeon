using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMusicPlayer_Singleton : MonoBehaviour
{
    private static FmodMusicPlayer_Singleton _instance = null;
    public static FmodMusicPlayer_Singleton Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
