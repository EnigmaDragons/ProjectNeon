using AeLa.EasyFeedback.UI.Interfaces;
using TMPro;

namespace AeLa.EasyFeedback.UI.TMP
{
	internal class TMPTextWrapper : UIInteropWrapper<TMP_Text>, IText
	{
		public string Text
		{
			get => InternalTarget.text;
			set => InternalTarget.text = value;
		}

		public TMPTextWrapper(TMP_Text internalTarget) : base(internalTarget) { }
	}
}
