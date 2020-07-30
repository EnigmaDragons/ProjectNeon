using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorBarLogic : MonoBehaviour {

    public RectTransform ParentBars;

    private RectTransform[] _bars;
    private float[] _widthBars;

	// Use this for initialization
	void Start () {
        _bars = new RectTransform[ParentBars.childCount];
        _widthBars = new float[ParentBars.childCount];
        for (int i = 0; i < ParentBars.childCount; i++)
        {
            _bars[i] = ParentBars.GetChild(i).GetComponent<RectTransform>();
            _widthBars[i] = _bars[i].sizeDelta.x;
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < _bars.Length; i++)
        {
            float width = (0.5f + 0.5f * Mathf.Sin(i + 3 * Time.time * Mathf.Sin(i))) * _widthBars[i];
            _bars[i].sizeDelta = new Vector2(width, _bars[i].sizeDelta.y);
        }
    }
}
