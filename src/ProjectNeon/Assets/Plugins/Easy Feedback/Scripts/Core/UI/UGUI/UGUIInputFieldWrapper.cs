using AeLa.EasyFeedback.UI.Interfaces;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AeLa.EasyFeedback.UI.UGUI
{
    internal class UGUIInputFieldWrapper
        : UIInteropWrapper<InputField>, IInputField
    {
        public UGUIInputFieldWrapper(InputField internalTarget)
            : base(internalTarget)
        {
        }

        public UnityEvent<string> OnValueChanged =>
            InternalTarget.onValueChanged;

        public string Text
        {
            get => InternalTarget.text;
            set => InternalTarget.text = value;
        }

        public bool IsFocused => InternalTarget.isFocused;

        public void ActivateInputField()
        {
            InternalTarget.ActivateInputField();
        }

        public void DeactivateInputField()
        {
            InternalTarget.DeactivateInputField();
        }
    }
}