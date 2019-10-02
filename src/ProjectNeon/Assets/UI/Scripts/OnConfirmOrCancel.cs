    using UnityEngine;
    using UnityEngine.Events;

    public sealed class OnConfirmOrCancel : MonoBehaviour
    {
        [SerializeField] private UnityEvent onSubmit;
        [SerializeField] private UnityEvent onCancel;
        
        private void Update()
        {
            if (Input.GetButtonDown("Submit"))
                onSubmit.Invoke();
            if (Input.GetButtonDown("Cancel"))
                onCancel.Invoke();
        }
    }
