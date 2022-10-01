using AeLa.EasyFeedback.UI.Interfaces;
using UnityEngine.UI;

namespace AeLa.EasyFeedback.UI.UGUI
{
	internal class UGUIDropdownWrapper 
		: UIInteropWrapper<Dropdown>, IDropdown
	{
		public IText CaptionText { get; private set; }

		public int Value
		{
			get => InternalTarget.value;
			set => InternalTarget.value = value;
		}

		public UGUIDropdownWrapper(Dropdown internalTarget) : base(internalTarget)
		{
			CaptionText = new UGUITextWrapper(internalTarget.captionText);
		}

		public void ClearOptions()
		{
			InternalTarget.ClearOptions();
		}

		public void RefreshShownValue()
		{
			InternalTarget.RefreshShownValue();
		}

		public void AddOption(string text)
		{
			InternalTarget.options.Add(new Dropdown.OptionData(text));
		}
	}
}