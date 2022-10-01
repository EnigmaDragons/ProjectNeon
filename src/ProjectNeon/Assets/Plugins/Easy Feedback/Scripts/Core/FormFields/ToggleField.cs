using System;
using UnityEngine;
using UnityEngine.UI;

// disable "Field is never assigned to and will have no value" for Editor-exposed properties
#pragma warning disable 0649, 1692

namespace AeLa.EasyFeedback.FormFields
{
    /// <summary>
    /// A basic implementation of a toggle (checkbox) field
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    class ToggleField : FormField
    {
        [Tooltip("The label to prepend to this field on the report (won't be included if left blank)")]
        public string Label;

        [Tooltip("The default value of the toggle")]
        public bool Default;

        private Toggle sourceField;

        public override void Awake()
        {
            base.Awake();

            // get source field
            sourceField = GetComponent<Toggle>();

            // set default value
            sourceField.isOn = Default;
        }

        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            // add section to the form
            if (string.IsNullOrEmpty(SectionTitle))
                throw new NullReferenceException("The section title for this field is not set!");

            if (!Form.CurrentReport.HasSection(SectionTitle))
                Form.CurrentReport.AddSection(SectionTitle, SortOrder);
            else
            {
                Debug.LogWarning("The section " + SectionTitle + " already exists! Overwriting.");
            }

            // set section text
            string sectionText;
            if (string.IsNullOrEmpty(Label))
            {
                sectionText = sourceField.isOn.ToString();
            }
            else
            {
                sectionText = string.Format("{0}: {1}", Label, sourceField.isOn);
            }

            Form.CurrentReport[SectionTitle].SetText(sectionText);
            Form.CurrentReport[SectionTitle].SortOrder = SortOrder; // ef-TODO: maybe we should move this to FormField's FormSubmitted

            // set default value
            sourceField.isOn = Default;
        }
    }
}
