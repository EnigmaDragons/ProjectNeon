using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AeLa.EasyFeedback.Demo
{
    [RequireComponent(typeof(Text))]
    public class CallbackTest : MonoBehaviour
    {
        public float FadeTime = 2f;

        private Text text;
        private Coroutine coroutine;

        void Start()
        {
            text = GetComponent<Text>();
            SetAlpha(0f);
        }

        private void SetAlpha(float a)
        {
            Color c = text.color;
            c.a = a;
            text.color = c;
        }

        /// <summary>
        /// Called by an event on the Feedback Form.
        /// </summary>
        public void OnEvent()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeCoroutine());
        }

        IEnumerator FadeCoroutine()
        {
            float a = 1f;

            do
            {
                SetAlpha(a);
                a -= Time.deltaTime / FadeTime;
                yield return new WaitForEndOfFrame();
            } while (a > 0);
        }
    }
}