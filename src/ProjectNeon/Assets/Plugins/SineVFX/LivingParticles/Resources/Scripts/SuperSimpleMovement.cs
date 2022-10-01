using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSimpleMovement : MonoBehaviour {

    public float movementSpeed = 3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * movementSpeed, 0f, Input.GetAxis("Vertical") * Time.deltaTime * movementSpeed);
	}
}
