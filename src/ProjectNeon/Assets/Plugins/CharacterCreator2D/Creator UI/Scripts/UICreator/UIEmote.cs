using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
	public class UIEmote : MonoBehaviour {

		public CharacterViewer character;
		public EmotionType activeEmotion;
		public Transform emotionList;
		public Transform emotionEditor;
		public Transform[] emoteMenu;

		void OnEnable() 
		{
			emotionEditor.gameObject.SetActive(false);
			emotionList.gameObject.SetActive(true);
		}

		void OnDisable()
		{
			if (character == null)
				return;
			character.ResetEmote();
			OpenPartMenu(emoteMenu[0].gameObject);
		}		

		public void OpenType (EmotionType emotionType) {
			activeEmotion = emotionType;
			character.Emote(activeEmotion);
			emotionList.gameObject.SetActive(false);
			emotionEditor.gameObject.SetActive(true);
			Text title = emotionEditor.Find("Title/Type").GetComponent<Text>();		
			string name = character.emotes.getIndex(emotionType).name;
			if (!string.IsNullOrEmpty(name))
				title.text = name;
			else	
				title.text = System.Enum.GetName(typeof(EmotionType), activeEmotion);
			InputField input = emotionEditor.Find("Menu/Name/Input").GetComponent<InputField>();
			input.text = character.emotes.getIndex(activeEmotion).name;
			if (activeEmotion >= EmotionType.Blink) 
				input.transform.parent.gameObject.SetActive(false);
			else 
				input.transform.parent.gameObject.SetActive(true);
		}
		
		public void SetEmotionName (string name) {
			character.emotes.getIndex(activeEmotion).name = name;
		}

		public void OpenPartMenu (GameObject partMenu)
		{			
			foreach (Transform t in emoteMenu)
			{
				t.gameObject.SetActive(false);
			}
			if (partMenu != null)
			partMenu.SetActive(true);
		}
	}
}