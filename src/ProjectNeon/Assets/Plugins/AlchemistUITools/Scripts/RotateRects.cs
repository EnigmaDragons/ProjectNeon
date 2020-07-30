using UnityEngine;
using System.Collections;

public class RotateRects : MonoBehaviour {

	[System.Serializable]
	public class RotObject
	{
		public RectTransform rect;
		public float speed;
        public bool isChangeDirr = false;
        public float frequency = 1;
	}

	public RotObject[] rotObjects;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < rotObjects.Length; i++) 
		{
            if(!rotObjects[i].isChangeDirr)
			    rotObjects [i].rect.Rotate (new Vector3 (0, 0, rotObjects [i].speed * Time.deltaTime));
            else
                rotObjects[i].rect.Rotate(new Vector3(0, 0, Mathf.Sin(Time.time * rotObjects[i].frequency * Mathf.PI * 2) * rotObjects[i].speed * Time.deltaTime));
        }
	}
}
