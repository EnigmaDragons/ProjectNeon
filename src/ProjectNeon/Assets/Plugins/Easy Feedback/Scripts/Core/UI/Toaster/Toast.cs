using UnityEngine;
using AeLa.EasyFeedback.UI.Interfaces;
using UnityEngine.Serialization;

namespace AeLa.EasyFeedback.UI.Toaster
{
    [RequireComponent(typeof(RectTransform))]
    public class Toast : MonoBehaviour
    {
        [FormerlySerializedAs("text")] [SerializeField]
        protected GameObject Text;

        private IText textComponent;

        public string Message
        {
            get => textComponent.Text;
            set => textComponent.Text = value;
        }

        public RectTransform RectTransform => (RectTransform) transform;

        private void Awake()
        {
            textComponent = UIInterop.GetText(Text);
        }
    }
}