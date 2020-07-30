using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransparentPeriod : MonoBehaviour {

    [System.Serializable]
    public class TransObject
    {
        public Image img;
        public Color first;
        public Color second;
        public float period = 1;
    }

    public TransObject[] transObjects;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    for(int i = 0; i < transObjects.Length; i++)
        {
            float koef = Mathf.Cos(Mathf.PI / transObjects[i].period * Time.time);
            transObjects[i].img.color = transObjects[i].first * koef + transObjects[i].second * (1 - koef);
        }
	}
}
