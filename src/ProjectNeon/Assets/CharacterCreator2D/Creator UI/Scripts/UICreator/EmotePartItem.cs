using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CharacterCreator2D.UI
{
	public class EmotePartItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler 
	{
		Part part;
		static Part oriPart;

		CharacterViewer character;		
		EmotePartGroup emotePartGroup;
		EmoteIndex emoteIndex;
		EmotionType emotionType;		

		public void Initialize (Part part)
		{			
			this.part = part;
            this.transform.name = part == null ? "NULL" : part.packageName + "_" + part.name;
            this.transform.Find("Text").GetComponent<Text>().text = part == null ? "NULL" : part.name;
		}

		void Start () {
			emotePartGroup = this.transform.GetComponentInParent<EmotePartGroup>();
			emoteIndex = emotePartGroup.emoteIndex;
			emotionType = emotePartGroup.emotionType;
			character = emotePartGroup.emoteUI.character;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			oriPart = emoteIndex.getPart(emotePartGroup.slotCategory);
			emoteIndex.setPart(emotePartGroup.slotCategory, part);
			character.ResetEmote();
			character.Emote(emotionType);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			emoteIndex.setPart(emotePartGroup.slotCategory, oriPart);
			character.ResetEmote();
			character.Emote(emotionType);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			emoteIndex.setPart(emotePartGroup.slotCategory, part);
			oriPart = part;	
			character.ResetEmote();
			character.Emote(emotionType);
			emotePartGroup.SelectItem (this);
		}
	}
}