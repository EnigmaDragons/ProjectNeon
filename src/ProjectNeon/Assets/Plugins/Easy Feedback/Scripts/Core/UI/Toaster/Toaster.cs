using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace AeLa.EasyFeedback.UI.Toaster
{
    /// <summary>
    /// Displays <see cref="Toast">Toasts</see>.
    /// </summary>
    public class Toaster : MonoBehaviour
    {
        /// <summary>
        /// The toast prefab object
        /// </summary>
        [FormerlySerializedAs("toastPrefab")]
        [Tooltip("The toast prefab object")]
        [SerializeField]
        protected Toast ToastPrefab;

        /// <summary>
        /// Where the toast will appear on screen
        /// </summary>
        [FormerlySerializedAs("viewportAnchor")]
        [Tooltip("Where the toast will appear on screen")]
        [SerializeField]
        protected ToastAnchor ViewportAnchor = ToastAnchor.TopRight;

        /// <summary>
        /// Direction the toast will move when it appears
        /// </summary>
        [FormerlySerializedAs("popupDirection")]
        [Tooltip("Direction the toast will move when it appears")]
        [SerializeField]
        protected PopoutDirection PopupDirection = PopoutDirection.Down;

        /// <summary>
        /// How long (seconds) a message remains on screen
        /// </summary>
        [FormerlySerializedAs("duration")]
        [Tooltip("How long (seconds) a message remains on screen")]
        [SerializeField]
        protected float Duration = 1.5f;

        /// <summary>
        /// How long (seconds) the slide in/out animation takes
        /// </summary>
        [FormerlySerializedAs("animationDuration")]
        [Tooltip("How long (seconds) the slide in/out animation takes")]
        [SerializeField]
        protected float AnimationDuration = 0.25f;

        /// <summary>
        /// Inactive toasts
        /// </summary>
        private List<Toast> inactive = new List<Toast>();

        /// <summary>
        /// Displays a toast with the provided message
        /// </summary>
        /// <param name="message"></param>
        public void Toast(string message) => StartCoroutine(ShowToast(message));

        private IEnumerator ShowToast(string message)
        {
            // get toast
            var toast = GetToast(message);
            var rt = toast.RectTransform;

            // init animation
            var speed = 1f / AnimationDuration;
            var direction = GetAnimationDirection(PopupDirection);
            var pivotIn = rt.pivot;
            var pivotOut = pivotIn - direction;

            // animate in
            yield return SlideAnim(rt, pivotIn, pivotOut, speed);

            // wait 
            yield return new WaitForSeconds(Duration);

            // animate out
            yield return SlideAnim(rt, pivotOut, pivotIn, speed);

            // return toast to pool
            ReturnToast(toast);
        }

        private IEnumerator SlideAnim(
            RectTransform rt, Vector2 from, Vector2 to, float speed
        )
        {
            for (var t = 0f; t <= 1f; t += Time.deltaTime * speed)
            {
                // ease out expo
                var e = t >= 1f ? 1f : 1f - Mathf.Pow(2, -10 * t);
                rt.pivot = Vector2.Lerp(from, to, e);
                yield return null;
            }
        }

        private Toast GetToast(string message)
        {
            if (inactive.Count == 0)
            {
                // need to add a new toast to the pool
                inactive.Add(Instantiate(ToastPrefab, transform));
            }

            // get toast from pool
            var toast = inactive[0];
            inactive.RemoveAt(0);

            var rt = toast.RectTransform;
            var pivot = rt.pivot;

            // apply anchor/pivots
            switch (ViewportAnchor)
            {
                case ToastAnchor.TopLeft:
                    rt.anchorMax = rt.anchorMin = new Vector2(0f, 1f);
                    pivot.x = 0;
                    pivot.y = 1;
                    break;
                case ToastAnchor.TopRight:
                    rt.anchorMax = rt.anchorMin = Vector2.one;
                    pivot.x = 1;
                    pivot.y = 1;
                    break;
                case ToastAnchor.BottomRight:
                    rt.anchorMax = rt.anchorMin = new Vector2(1f, 0f);
                    pivot.x = 1;
                    pivot.y = 0;
                    break;
                case ToastAnchor.BottomLeft:
                    rt.anchorMax = rt.anchorMin = Vector2.zero;
                    pivot.x = 0;
                    pivot.y = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (PopupDirection)
            {
                case PopoutDirection.Up:
                    pivot.y = 1;
                    break;
                case PopoutDirection.Down:
                    pivot.y = 0;
                    break;
                case PopoutDirection.Right:
                    pivot.x = 1;
                    break;
                case PopoutDirection.Left:
                    pivot.x = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rt.pivot = pivot;
            rt.anchoredPosition = new Vector2(0f, 0f);

            // set message
            toast.Message = message;

            // set visibility
            toast.gameObject.SetActive(true);

            return toast;
        }

        private void ReturnToast(Toast toast)
        {
            toast.gameObject.SetActive(false);
            inactive.Add(toast);
        }

        private Vector2 GetAnimationDirection(PopoutDirection direction)
        {
            switch (direction)
            {
                case PopoutDirection.Up: return Vector2.up;
                case PopoutDirection.Down: return Vector2.down;
                case PopoutDirection.Right: return Vector2.right;
                case PopoutDirection.Left: return Vector2.left;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(direction), direction, null
                    );
            }
        }

        public enum PopoutDirection
        {
            Up,
            Down,
            Right,
            Left
        }

        public enum ToastAnchor
        {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
        }
    }
}