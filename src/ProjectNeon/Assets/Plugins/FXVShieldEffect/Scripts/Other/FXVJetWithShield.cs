using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXV
{
    public class FXVJetWithShield : MonoBehaviour
    {
        public FXVShield shield;

        void Start()
        {
            shield.SetShieldEffectDirection((transform.right + transform.up).normalized);
        }

        void Update()
        {

        }
    }
}
