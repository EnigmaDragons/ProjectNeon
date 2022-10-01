using UnityEngine;

namespace AeLa.EasyFeedback
{
    /// <summary>
    /// Manages a field on the <see cref="FeedbackForm"/>
    /// </summary>
    public abstract class FormField : FormElement
    {
        /// <summary>
        /// The title of this field's section on the report
        /// </summary>
        [Tooltip("The title of this field's section on the report")]
        public string SectionTitle;

        /// <summary>
        /// Order of the section in the report (lowest first)
        /// </summary>
        [Tooltip("Order of the section in the report (lowest first)")]
        public int SortOrder;
    }
}