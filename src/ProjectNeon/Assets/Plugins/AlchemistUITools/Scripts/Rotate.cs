using UnityEngine;
using System.Collections;

namespace UITool
{
    public class Rotate : MonoBehaviour
    {

        public float speedRot;
       
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(0, speedRot * Time.deltaTime, 0));
        }
    }
}
