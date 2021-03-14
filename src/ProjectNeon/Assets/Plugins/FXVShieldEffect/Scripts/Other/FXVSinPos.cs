using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXV
{
    public class FXVSinPos : MonoBehaviour
    {
        public float amplitude = 2.0f;
        public float speed = 1.0f;

        private Vector3 startPos;

        private float angle = 0.0f;

        void Start()
        {
            startPos = transform.position;
        }

        void Update()
        {
            angle += Time.deltaTime * speed;
            if (angle > Mathf.PI * 2.0f)
                angle -= Mathf.PI * 2.0f;

            transform.position = startPos + Vector3.up * Mathf.Sin(angle) * amplitude;
        }
    }

}