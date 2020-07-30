using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AlchemistUISciFi
{
	public class UpgradeCategory
	{
		private RectTransform rect;
		private Vector2 startPos;
		public Vector2 aimPos;
		private Vector2 startSize;
		private Vector2 aimSize;
		
		public UpgradeCategory(Transform transform, UnityAction onClick)
		{
			rect = transform.GetComponent<RectTransform>();
			transform.GetComponent<Button>().onClick.AddListener(onClick);
			startPos = rect.anchoredPosition;
			aimPos = startPos;
			startSize = rect.sizeDelta;
			aimSize = startSize;
		}
		
		public void Update(float deltaTime)
		{
			rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, aimPos, deltaTime * 3f);
			rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, aimSize, deltaTime * 3f);
		}
		
		public void Reset()
		{
			aimPos = startPos;
			aimSize = startSize;
		}
		
		public void SetLittle()
		{
			aimSize = startSize * 0.55f;
		}
	}
}
