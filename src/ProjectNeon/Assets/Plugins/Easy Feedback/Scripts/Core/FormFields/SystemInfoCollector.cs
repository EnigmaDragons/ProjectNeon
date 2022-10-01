using UnityEngine;

namespace AeLa.EasyFeedback.FormFields
{
    class SystemInfoCollector : FormField
    {
        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            // add section to report if it doesn't already exist
            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);

            // append system info to section
            Form.CurrentReport["System Info"].AppendLine("OS: " + SystemInfo.operatingSystem);
            Form.CurrentReport["System Info"].AppendLine("Processor: " + SystemInfo.processorType);
            Form.CurrentReport["System Info"].AppendLine("Memory: " + SystemInfo.systemMemorySize);
            Form.CurrentReport["System Info"].AppendLine("Graphics API: " + SystemInfo.graphicsDeviceType);
            Form.CurrentReport["System Info"].AppendLine("Graphics Processor: " + SystemInfo.graphicsDeviceName);
            Form.CurrentReport["System Info"].AppendLine("Graphics Memory: " + SystemInfo.graphicsMemorySize);
            Form.CurrentReport["System Info"].AppendLine("Graphics Vendor: " + SystemInfo.graphicsDeviceVendor);
        }
    }

}