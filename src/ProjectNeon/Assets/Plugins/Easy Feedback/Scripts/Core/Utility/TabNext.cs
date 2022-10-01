using AeLa.EasyFeedback.UI;
using AeLa.EasyFeedback.UI.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace AeLa.EasyFeedback.Utility
{
	public class TabNext : MonoBehaviour
	{
		public Selectable Next;
		public Selectable Previous;

		/// <summary>
		/// Attached InputField (TMP or Unity)
		/// </summary>
		private IInputField input;

		// InputField (TMP or Unity) attached to Next and Previous, respectively
		private IInputField nextInput;
		private IInputField previousInput;

		private void Start()
		{
			// get attached input field(s)
			input = UIInterop.GetInputField(gameObject);

			if (Next)
			{
				nextInput = UIInterop.GetInputField(Next.gameObject, true);
			}

			if (Previous)
			{
				previousInput = UIInterop.GetInputField(Previous.gameObject, true);
			}
		}

		void Update()
		{
			if (
				Next != null && input.IsFocused
				&& !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				&& Input.GetKeyDown(KeyCode.Tab)
			)
			{
				input.DeactivateInputField();
				Next.Select();

				if (nextInput != null)
				{
					nextInput.ActivateInputField();
				}
			}
			else if (
				Previous != null && input.IsFocused
				&& (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				&& Input.GetKeyDown(KeyCode.Tab)
			)
			{
				input.DeactivateInputField();
				Previous.Select();

				if (previousInput != null)
				{
					previousInput.ActivateInputField();
				}
			}
		}
	}
}