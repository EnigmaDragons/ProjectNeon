using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AeLa.EasyFeedback.Utility
{
    [RequireComponent(typeof(FormElement))]
    public class SetSelectedOnOpen : MonoBehaviour
    {
        private FeedbackForm form;
        private Coroutine coroutine;

        private void Awake()
        {
            form = GetComponentInParent<FeedbackForm>();

            // register event handlers
            form.OnFormOpened.AddListener(StartSelectedCoroutine);
            form.OnFormClosed.AddListener(StopCoroutineIfExists);
        }

        private void StartSelectedCoroutine()
        {
            coroutine = StartCoroutine(SetSelfAsSelected());
        }

        private void StopCoroutineIfExists()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        IEnumerator SetSelfAsSelected()
        {
            // check if there is an eventsystem in the scene
            if (!EventSystem.current)
            {
                Debug.LogError("Scene is missing an EventSystem.");
                yield break;
            }

            // set self as selected
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(this.gameObject, null);

            coroutine = null;
        }
    }
}