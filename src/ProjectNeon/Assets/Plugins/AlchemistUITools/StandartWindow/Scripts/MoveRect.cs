using UnityEngine;
using System.Collections;

public class MoveRect : MonoBehaviour {

    [System.Serializable]
    public class MoveObject
    {
        public RectTransform rect;
        public Transform first;
        public Transform second;
        public float firstProcent = 0;
        public float secondProcent = 100;
        public float period = 1;
    }

    public MoveObject[] moveObjects;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < moveObjects.Length; i++)
        {
            float koef = 0.5f + Mathf.Sin(Time.time / moveObjects[i].period * 2 * Mathf.PI) / 2f;
            koef = moveObjects[i].firstProcent / 100f + koef * (moveObjects[i].secondProcent - moveObjects[i].firstProcent) / 100f;
            moveObjects[i].rect.position = Vector3.Lerp(moveObjects[i].first.position, moveObjects[i].second.position, koef);
            //Debug.Log("koef = " + koef);
        }

    }
}
