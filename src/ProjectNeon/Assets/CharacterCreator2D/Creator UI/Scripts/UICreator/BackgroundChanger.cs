using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
	public class BackgroundChanger : MonoBehaviour {

		public Image backgroundUI;
		public Sprite[] backgroundSprites;
		public int selectedBackground = 0;

		void Start () {
			if (backgroundUI == null || backgroundSprites == null)
				return;
			selectedBackground = PlayerPrefs.GetInt("Simpleton/CC2D/Background", 0);
			backgroundUI.sprite = backgroundSprites[selectedBackground];
		}

		public void NextBackground() {
			if (backgroundUI == null || backgroundSprites == null)
				return;
			selectedBackground += 1;
			if (selectedBackground >= backgroundSprites.Length)
				selectedBackground = 0;
			backgroundUI.sprite = backgroundSprites[selectedBackground];
			PlayerPrefs.SetInt("Simpleton/CC2D/Background", selectedBackground);
		}	
	}
}