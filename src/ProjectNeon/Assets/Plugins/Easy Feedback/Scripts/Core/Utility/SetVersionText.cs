using AeLa.EasyFeedback.UI;
using UnityEngine;

namespace AeLa.EasyFeedback.Utility
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class SetVersionText : MonoBehaviour
    {
        public string VersionNumber;
        public string Prefix, Suffix;

        // Use this for initialization
        private void Start()
        {
            // set version text
            var text = UIInterop.GetText(gameObject);
            text.Text = Prefix + VersionNumber + Suffix;
        }
    }
}