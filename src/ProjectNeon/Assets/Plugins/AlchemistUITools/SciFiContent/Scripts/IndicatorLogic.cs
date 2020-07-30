using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIHealthAlchemy;

namespace AlchemistUISciFi
{
	public class IndicatorLogic : MonoBehaviour {

		public ElemsHealthBar blueBar;
		public ElemsHealthBar orangeBar;
		public bool isTest = false;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if (isTest)
			{
				blueBar.Value = (1 + 1.1f * Mathf.Cos(1f * Time.time)) / 2;
				orangeBar.Value = (1 + 1.1f * Mathf.Cos(1f * Time.time)) / 2;
			}
		}
	}
}
