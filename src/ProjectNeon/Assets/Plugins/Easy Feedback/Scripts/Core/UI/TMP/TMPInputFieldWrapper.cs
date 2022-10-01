using AeLa.EasyFeedback.UI.Interfaces;
using TMPro;
using UnityEngine.Events;

namespace AeLa.EasyFeedback.UI.TMP
{
	internal class TMPInputFieldWrapper
		: UIInteropWrapper<TMP_InputField>, IInputField
	{
		public TMPInputFieldWrapper(TMP_InputField internalTarget) : base(internalTarget) { }

		public UnityEvent<string> OnValueChanged => InternalTarget.onValueChanged;

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