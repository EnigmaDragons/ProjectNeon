using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CharacterCreator2D.UI
{
	public class DropdownShifter : MonoBehaviour, IPointerClickHandler {

		public Dropdown dropdown;
		public bool next;
		public bool previous;
		
		public void OnPointerClick (PointerEventData eventData) {
			if (next) Next();
			else if (previous) Previous();
		}
		
		void Next () {
			if (dropdown.value >= dropdown.options.Count - 1)
				dropdown.value = 0;
			else
				dropdown.value += 1;
		}

		void Previous () {
			if (dropdown.value <= 0)
				dropdown.value = dropdown.options.Count - 1;
			else
				dropdown.value -= 1;
		}
	}
}
