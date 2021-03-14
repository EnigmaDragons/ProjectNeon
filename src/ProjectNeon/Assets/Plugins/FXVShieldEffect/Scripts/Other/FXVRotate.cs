using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXV
{
    public class FXVRotate : MonoBehaviour
    {

        public Vector3 rotationSpeed = Vector3.up;

        private Vector3 currentRotation;

        void Start()
        {
            currentRotation = transform.rotation.eulerAngles;
        }

        void Update()
        {
            currentRotation += rotationSpeed * Time.deltaTime;

            transform.rotation = Quaternion.identity;
            transform.Rotate(currentRotation);
        }
    }
}
