using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
	[CreateAssetMenu(fileName = "AnimList", menuName = "CC2D/AnimList")]
	public class AnimationList : ScriptableObject {
		public List<string> stateName;
	}
}
