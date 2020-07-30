using UnityEngine;
using System.Collections;

public class ScaleRect : MonoBehaviour {

    [System.Serializable]
    public class ScaleObject
    {
        public RectTransform rect;
        public Vector3 first;
        public Vector3 second;
        public float period = 1;
    }

    public ScaleObject[] scaleObjects;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < scaleObjects.Length; i++)
        {
            float koef = 0.5f + Mathf.Sin(Time.time / scaleObjects[i].period * 2 * Mathf.PI) / 2f;
            scaleObjects[i].rect.localScale = Vector3.Lerp(scaleObjects[i].first, scaleObjects[i].second, koef);
            //Debug.Log("koef = " + koef);
        }

    }
}
