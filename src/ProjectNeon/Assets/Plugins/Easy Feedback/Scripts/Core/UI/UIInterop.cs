using System;
using AeLa.EasyFeedback.UI.Interfaces;
using AeLa.EasyFeedback.UI.TMP;
using AeLa.EasyFeedback.UI.UGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AeLa.EasyFeedback.UI
{
	/// <summary>
	/// Utility that allows agnostic support for 
	/// TMP Text or UGUI Text in Easy Feedback scripts.
	/// </summary>
	internal static class UIInterop
	{
		public static IText GetText(GameObject go)
		{
			if (TMPTextWrapper.GetTarget(go))
			{
				return new TMPTextWrapper(TMPTextWrapper.GetTarget(go));
			}
			else if (UGUITextWrapper.GetTarget(go))
			{
				return new UGUITextWrapper(UGUITextWrapper.GetTarget(go));
			}

			// TODO: better exception -ntr
			throw GetNonCompatibleException("Text", go);
		}

		internal static IDropdown GetDropdown(GameObject gameObject)
		{
			if (TMPDropdownWrapper.GetTarget(gameObject))
			{
				return new TMPDropdownWrapper(TMPDropdownWrapper.GetTarget(gameObject));
			}
			else if (UGUIDropdownWrapper.GetTarget(gameObject))
			{
				return new UGUIDropdownWrapper(UGUIDropdownWrapper.GetTarget(gameObject));
			}

			throw GetNonCompatibleException("Dropdown", gameObject);
		}

		internal static IInputField GetInputField(GameObject gameObject, bool soft = false)
		{
			if (TMPInputFieldWrapper.GetTarget(gameObject))
			{
				return new TMPInputFieldWrapper(TMPInputFieldWrapper.GetTarget(gameObject));
			}
			else if (UGUIInputFieldWrapper.GetTarget(gameObject))
			{
				return new UGUIInputFieldWrapper(UGUIInputFieldWrapper.GetTarget(gameObject));
			}

			if (soft)
			{
				return null;
			}
			else
			{
				throw GetNonCompatibleException("InputField", gameObject);
			}
		}

		/// <summary>
		/// Returns an <see cref="IText"/> object for either
		/// <paramref name="unityText"/> or <paramref name="tmpText"/>,
		/// whichever is not null. Returns null if both are null.
		/// </summary>
		/// <param name="unityText"></param>
		/// <param name="tmpText"></param>
		/// <returns></returns>
		internal static IText GetText(Text unityText, TMP_Text tmpText)
		{
			if (unityText) return new UGUITextWrapper(unityText);
			else if (tmpText) return new TMPTextWrapper(tmpText);

			return null;
		}

		private static Exception GetNonCompatibleException(string elementType, GameObject go)
		{
			// TODO: better exception -ntr
			return new Exception(
				"Could not find a " + elementType + " component attached to " + go
			);
		}
	}
}