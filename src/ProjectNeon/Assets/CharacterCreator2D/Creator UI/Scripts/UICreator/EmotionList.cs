using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
	public class EmotionList : MonoBehaviour 
	{
		public EmotionTypeItem emotionTypeItem;
		public EmotionTypeItem emotionSpacer;
		UIEmote emoteUI;
		Transform itemParent;

		void Awake()
		{
			emoteUI = GetComponentInParent<UIEmote>();
			itemParent = this.transform.Find("List/Content");
		}

		void OnEnable () 
		{
			Clear();
			Initialize();
		}
		
		void Initialize () 
		{
			emoteUI.character.ResetEmote();
			EmotionTypeItem spacer = Instantiate<EmotionTypeItem>(emotionSpacer, itemParent);
			spacer.Initialize("PRESET");
			foreach (EmotionType e in Enum.GetValues(typeof(EmotionType)))
			{
				if (e < EmotionType.Blink)
					continue;
				EmotionTypeItem item = Instantiate<EmotionTypeItem>(emotionTypeItem, itemParent);
				item.Initialize(e);	
			}
			spacer = Instantiate<EmotionTypeItem>(emotionSpacer, itemParent);
			spacer.Initialize("CUSTOM");	
			foreach (EmotionType e in Enum.GetValues(typeof(EmotionType)))
			{
				if (e >= EmotionType.Blink)
					return;
				EmotionTypeItem item = Instantiate<EmotionTypeItem>(emotionTypeItem, itemParent);
				item.Initialize(e);	
			}		
		}

		void Clear () 
		{
			foreach (Transform t in itemParent) Destroy(t.gameObject);
		}
	}
}

