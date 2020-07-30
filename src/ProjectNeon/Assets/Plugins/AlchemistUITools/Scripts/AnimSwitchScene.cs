using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimSwitchScene : MonoBehaviour {

    private static AnimSwitchScene _instance;

    public float intervalStart = 1;
    public float intervalEnd = 3;

    private int indexLevel = -1;
    private Image _img;
    private float _timeEnd = -1;

    public static void LoadLevel(int index)
    {
        if (_instance)
            _instance._LoadLevel(index);
        else
            Application.LoadLevel(index);
    }

    public void _LoadLevel(int index)
    {
        indexLevel = index;
        _timeEnd = Time.time;
    }

    // Use this for initialization
    void Awake () {
        _instance = this;
        _img = GetComponent<Image>();
        _img.enabled = true;
        _timeEnd = -1;
    }
	
	// Update is called once per frame
	void Update () {
        bool isActive = Time.timeSinceLevelLoad < intervalStart || (_timeEnd > 0 && Time.time > _timeEnd);
        _img.enabled = isActive;
        if(isActive)
        {
            Color col = _img.color;
            if (Time.timeSinceLevelLoad < intervalStart)
                col.a = (intervalStart - Time.timeSinceLevelLoad) / intervalStart;
            else
                col.a = (Time.time - _timeEnd) / intervalEnd;
            _img.color = col;
        }
        if (indexLevel >= 0 && Time.time >= _timeEnd + intervalEnd)
            Application.LoadLevel(indexLevel);
    }
}
