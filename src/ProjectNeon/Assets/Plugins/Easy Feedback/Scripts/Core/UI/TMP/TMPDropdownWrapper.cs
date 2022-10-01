using AeLa.EasyFeedback.UI.Interfaces;
using TMPro;

namespace AeLa.EasyFeedback.UI.TMP
{
	internal class TMPDropdownWrapper 
		: UIInteropWrapper<TMP_Dropdown>, IDropdown
	{
		public IText CaptionText { get; private set; }

		public int Value
		{
			get => InternalTarget.value;
			set => InternalTarget.value = value;
		}

		public TMPDropdownWrapper(TMP_Dropdown internalTarget) : base(internalTarget)
		{
			CaptionText = new TMPTextWrapper(internalTarget.captionText);
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
			InternalTarget.options.Add(new TMP_Dropdown.OptionData(text));
		}
	}
}