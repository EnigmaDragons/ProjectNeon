using UnityEngine;

namespace AeLa.EasyFeedback.FormFields
{
    class PlayerInfoCollector : FormField
    {
        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            // include player position if player exists
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            // add section to report if it doesn't already exist
            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);

            // add player info to report
            Form.CurrentReport["Additional Info"].AppendLine("Player Position: " + player.transform.position.ToString());
        }
    }

}