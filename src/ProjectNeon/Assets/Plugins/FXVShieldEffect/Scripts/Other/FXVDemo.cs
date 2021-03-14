using UnityEngine;
using System.Collections;

namespace FXV
{
    public class FXVDemo : MonoBehaviour
    {
        public bool allowMoveCamera = false;

        private Vector3 lastMousePos;
        private Vector3 targetCameraPos;

        private Vector3 startCameraDir;
        private Vector3 startCameraUp;
        private Vector3 startCameraRight;
        private Vector3 cameraDir;

        private int position = 0;
        void Start()
        {
            targetCameraPos = Camera.main.transform.position;

            cameraDir = startCameraDir = Camera.main.transform.forward;
            startCameraUp = Camera.main.transform.up;
            startCameraRight = Camera.main.transform.right;
        }

        void Update()
        {
            if (allowMoveCamera)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    lastMousePos = Input.mousePosition;
                }

                Vector3 wishCamDir = startCameraDir;

                if (Input.GetMouseButton(1))
                {
                    Vector3 deltaPos = Input.mousePosition - lastMousePos;

                    wishCamDir += startCameraUp * deltaPos.y * 0.005f + startCameraRight * deltaPos.x * 0.02f;
                }

                cameraDir = Vector3.Lerp(cameraDir, wishCamDir, Time.deltaTime * 2.0f);
                cameraDir.Normalize();

                Camera.main.transform.rotation = Quaternion.LookRotation(cameraDir);


                if (Input.GetKeyDown("left") && position > 0)
                {
                    targetCameraPos -= startCameraRight * 13.0f;
                    position--;
                }

                if (Input.GetKeyDown("right") && position < 4)
                {
                    targetCameraPos += startCameraRight * 13.0f;
                    position++;
                }

                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPos, Time.deltaTime * 5.0f);
            }

            if (Input.GetKeyDown("space"))
            {
                FXVShield[] shields = GameObject.FindObjectsOfType<FXVShield>();
                foreach (FXVShield s in shields)
                {
                    if (s.GetIsShieldActive())
                        s.SetShieldActive(false);
                    else
                        s.SetShieldActive(true);

                }
            }
        }

        public void EnableShields()
        {
            FXVShield[] shields = GameObject.FindObjectsOfType<FXVShield>();
            foreach (FXVShield s in shields)
                s.SetShieldActive(true);
        }

        public void DisableShiedls()
        {
            FXVShield[] shields = GameObject.FindObjectsOfType<FXVShield>();
            foreach (FXVShield s in shields)
                s.SetShieldActive(false);
        }
    }

}