using UnityEngine;

namespace AeLa.EasyFeedback.Demo
{
    public class CauseException : MonoBehaviour
    {
        public void ThrowException()
        {
            throw new System.Exception("Test");
        }
    }
}
