using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCase_Cam_Mov : MonoBehaviour
{
    public Vector3 axis;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Time.deltaTime * axis);
    }
}
