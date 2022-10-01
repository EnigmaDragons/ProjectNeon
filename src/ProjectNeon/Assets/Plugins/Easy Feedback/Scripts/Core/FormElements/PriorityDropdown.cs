using System.Collections.Generic;
using AeLa.EasyFeedback.UI;
using AeLa.EasyFeedback.UI.Interfaces;
using System.Linq;
using AeLa.EasyFeedback.APIs;

namespace AeLa.EasyFeedback.FormElements
{
    public class PriorityDropdown : FormElement
    {
        private IDropdown priorityDropdown;
        private IEnumerable<Label> labels;

        public override void Awake()
        {
            base.Awake();
            priorityDropdown = UIInterop.GetDropdown(gameObject);

            // add options
            priorityDropdown.ClearOptions();
            labels = Form.Config.Board.Labels.OrderBy(l => l.order);
            foreach(var label in labels)
                priorityDropdown.AddOption(label.name);

            priorityDropdown.Value = 0;
            priorityDropdown.RefreshShownValue();
        }

        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            // add the selected label to the report
            Form.CurrentReport.AddLabel(
                labels.ElementAt(priorityDropdown.Value)
            );
        }
    }
}