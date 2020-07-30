using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleElemRect : MonoBehaviour {

    public RectTransform rectParent;
    public Vector2 parentSize = Vector2.one;
    public Vector2 currSize = Vector2.one;

    private RectTransform _rect;

	// Use this for initialization
	void Start () {
        _rect = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        float sizeKoef = Mathf.Min(1f * rectParent.rect.width / parentSize.x, rectParent.rect.height / parentSize.y);
        _rect.sizeDelta = new Vector2(sizeKoef * currSize.x, sizeKoef * currSize.y);
    }
}
