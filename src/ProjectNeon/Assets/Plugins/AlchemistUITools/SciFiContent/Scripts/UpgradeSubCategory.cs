using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AlchemistUISciFi
{
	public class UpgradeSubCategory
	{
		public float aimAngle;
		public float startAngle;
		
		private RectTransform rect;
		private float currAngle;
		
		public UpgradeSubCategory(Transform transform,  UnityAction onClick)
		{
			rect = transform.GetComponent<RectTransform>();
			aimAngle = currAngle = startAngle = rect.rotation.eulerAngles.z;
			transform.GetChild(0).GetComponent<Button>().onClick.AddListener(onClick);
		}
		
		public void Update(float deltaTime)
		{
			if(Mathf.Abs(currAngle - aimAngle) > 0.5f)
			{
                while (aimAngle - currAngle > 180)
                    currAngle += 360;
                while (aimAngle - currAngle < -180)
                    currAngle -= 360;
                rect.rotation = Quaternion.identity;
				currAngle = Mathf.Lerp(currAngle, aimAngle, deltaTime * 3f);
				rect.Rotate(new Vector3(0, 0, currAngle));
				rect.GetChild(0).rotation = Quaternion.identity;
				
			}
			
		}
	}
}
