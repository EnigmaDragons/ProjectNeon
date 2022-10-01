using AeLa.EasyFeedback.UI;
using AeLa.EasyFeedback.UI.Interfaces;

namespace AeLa.EasyFeedback.FormElements
{
    public class CategoryDropdown : FormElement
    {
        private IDropdown typeDropdown;

        public override void Awake()
        {
            base.Awake();
            typeDropdown = UIInterop.GetDropdown(gameObject);

            // add options
            typeDropdown.ClearOptions();
            for (int i = 0; i < Form.Config.Board.CategoryNames.Length; i++)
            {
                typeDropdown.AddOption(Form.Config.Board.CategoryNames[i]);
            }
            typeDropdown.Value = 0;
            typeDropdown.RefreshShownValue();
        }

        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            Form.CurrentReport.List.id = Form.Config.Board.CategoryIds[typeDropdown.Value];
            Form.CurrentReport.List.name = Form.Config.Board.CategoryNames[typeDropdown.Value];
        }
    }
}