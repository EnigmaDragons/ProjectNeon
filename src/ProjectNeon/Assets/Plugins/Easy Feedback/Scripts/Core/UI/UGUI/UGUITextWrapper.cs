using AeLa.EasyFeedback.UI.Interfaces;
using UnityEngine.UI;

namespace AeLa.EasyFeedback.UI.UGUI
{
	internal class UGUITextWrapper : UIInteropWrapper<Text>, IText
	{
		public string Text
		{
			get => InternalTarget.text;
			set => InternalTarget.text = value;
		}

		public UGUITextWrapper(Text internalTarget) : base(internalTarget) { }
	}
}
