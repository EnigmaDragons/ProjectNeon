using UnityEngine;

namespace AeLa.EasyFeedback.FormFields
{
    class VersionInfoCollector : FormField
    {
        [SerializeField] private StringReference versionString;
        [SerializeField] private BoolVariable isDemo;
        
        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);

            var suffix = "";
            if (isDemo)
                suffix = " Demo";
            #if UNITY_EDITOR
                suffix += " Editor";
            #endif
            
            Form.CurrentReport[SectionTitle].AppendLine("Version: " + versionString + suffix);
        }
    }
}
