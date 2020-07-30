using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleElem : MonoBehaviour {

    public RectTransform rectParent;
    public Vector2 parentSize = Vector2.one;
    public Vector2 currSize = Vector2.one;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        float sizeKoef = Mathf.Min(1f * rectParent.rect.width / parentSize.x, rectParent.rect.height / parentSize.y);
        transform.localScale = sizeKoef * Vector3.one;
    }
}
