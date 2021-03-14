using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FXV
{
    public class FXVResetTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name + " triggered me");

            other.transform.GetComponent<FXVCharacter>().ResetPosition();
        }
    }
}
