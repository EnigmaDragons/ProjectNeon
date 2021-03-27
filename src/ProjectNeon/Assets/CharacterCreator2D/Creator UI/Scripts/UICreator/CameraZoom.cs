using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreator2D.UI
{
	public class CameraZoom : MonoBehaviour 
	{
		[Header("UI")]
		public Slider zoomSlider;
		public Button zoomResetButton;

		[Header("Camera Zoom")]
		public float minSize;
		public float maxSize;
		public float defaultSize;

		[Header("Camera Position")]
		public float minY;
		public float maxY;

		Camera cam;
		Vector3 pos;

		void Awake () 
		{
			cam = GetComponent<Camera>();
			pos = cam.transform.position;
			zoomSlider.onValueChanged.AddListener(UpdateZoom);
			zoomResetButton.onClick.AddListener(ResetZoom);
			ResetZoom();
		}

		public void UpdateZoom (float value)
		{
			cam.orthographicSize = Mathf.Lerp(maxSize, minSize, value);
			if (value >= 0.5f)
				pos.y = Mathf.Lerp(minY, maxY, (value - 0.5f) * 2);
			else
				pos.y = minY;
			cam.transform.position = pos;
		}

		public void ResetZoom ()
		{
			zoomSlider.value = 1 - Mathf.InverseLerp(minSize, maxSize, defaultSize);
		}

		public void MaxZoom ()
		{
			zoomSlider.value = 1;
		}
	}
}