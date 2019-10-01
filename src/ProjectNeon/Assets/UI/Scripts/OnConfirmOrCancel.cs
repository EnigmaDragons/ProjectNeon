    using UnityEngine;
    using UnityEngine.Events;

    public sealed class OnConfirmOrCancel : MonoBehaviour
    {
        [ReadOnly, SerializeField] private bool isActive;
        [SerializeField] private GameEvent activateOn;
        [SerializeField] private GameEvent deactivateOn;
        [SerializeField] private UnityEvent onSubmit;
        [SerializeField] private UnityEvent onCancel;
        
        private void OnEnable()
        {
            isActive = false;
            activateOn.Subscribe(() => isActive = true, this);
            deactivateOn.Subscribe(() => isActive = false, this);
        }

        private void OnDisable()
        {
            activateOn.Unsubscribe(this);
            deactivateOn.Unsubscribe(this);
        }

        private void Update()
        {
            if (!isActive)
                return;

            if (Input.GetButtonDown("Submit"))
                onSubmit.Invoke();
            if (Input.GetButtonDown("Cancel"))
                onCancel.Invoke();
        }
    }
