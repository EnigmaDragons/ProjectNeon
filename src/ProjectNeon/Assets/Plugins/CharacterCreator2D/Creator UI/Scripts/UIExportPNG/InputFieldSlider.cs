using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CharacterCreator2D.UI 
{
	public class InputFieldSlider : MonoBehaviour {

		public InputField inputField;
		public Slider slider;
		public string stringFormat = "0.00";
		public Button resetButton;
		
		public ValueChange onValueChanged;

		float defaultValue;
		bool isEditingField;
		bool isEditingSlider;

		System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("");
		
		void Start () {	
			ci.NumberFormat.NumberDecimalSeparator = ".";
			if (inputField!=null) {
				inputField.text = slider.value.ToString(stringFormat, ci);
				inputField.onValueChanged.AddListener(FieldChange);
				inputField.onEndEdit.AddListener(EndEdit);
			}		
			if (slider!=null) {
				defaultValue = slider.value;
				slider.onValueChanged.AddListener(SliderChange);
			}
			if (resetButton!=null) {
				resetButton.onClick.AddListener(Reset);
			}
		}
		
		void SliderChange (float v) {
			if (isEditingField) 
				return;
			isEditingSlider = true;
			inputField.text = v.ToString(stringFormat, ci);
			onValueChanged.Invoke(v);
			isEditingSlider = false;
		}

		void FieldChange (string s) {
			if (isEditingSlider)
				return;
			isEditingField = true;
			float f = 0;
			if(float.TryParse(s, System.Globalization.NumberStyles.Float, ci, out f))
			{
				slider.value = f;
				onValueChanged.Invoke(slider.value);
			}
			isEditingField = false;
		}

		void EndEdit (string s) {
			float f = 0;
			if (float.TryParse(s, System.Globalization.NumberStyles.Float, ci, out f))
			{
				slider.value = f;
				inputField.text = slider.value.ToString(stringFormat, ci);
				onValueChanged.Invoke(slider.value);
			}
			else
				inputField.text = stringFormat;
			isEditingField = false;
		}

		void Reset () {
			slider.value = defaultValue;
		}
	}

	[System.Serializable]
	public class ValueChange : UnityEvent<float> {}
}
