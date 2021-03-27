using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
	public class AimAngle : MonoBehaviour {

		public Slider aimSlider;
		public float aimIncrement = 0.1f;
		public Animator characterViewer;

		public void UpdateAim () {
			characterViewer.SetFloat("Aim", aimSlider.value);
		}

		public void Increase () {
			aimSlider.value += aimIncrement;
			UpdateAim();
		}

		public void Decrease () {
			aimSlider.value -= aimIncrement;
			UpdateAim();
		}

		public void Reset () {
			aimSlider.value = 0;
			UpdateAim();
		}

	}
}
