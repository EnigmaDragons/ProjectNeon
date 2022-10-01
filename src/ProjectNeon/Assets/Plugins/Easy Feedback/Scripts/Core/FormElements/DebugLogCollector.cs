using System.Text;
using UnityEngine;

namespace AeLa.EasyFeedback.FormElements
{
    /// <summary>
    /// Attaches the debug log to the report as as text file
    /// </summary>
    class DebugLogCollector : FormElement
    {
        private StringBuilder log;

        public override void Awake()
        {
            base.Awake();

            // instantiate string builder
            log = new StringBuilder();

            // register Application log callback
            Application.logMessageReceived += HandleLog;
        }

        protected override void FormClosed()
        {
        }

        protected override void FormOpened()
        {
        }

        protected override void FormSubmitted()
        {
            // attach log
            byte[] bytes = Encoding.ASCII.GetBytes(log.ToString());
            Form.CurrentReport.AttachFile("log.txt", bytes);
        }

        private void HandleLog(
            string logString, string stackTrace, LogType logType
        )
        {
            // enqueue the message
            if (logType != LogType.Exception)
            {
                log.AppendFormat("{0}: {1}", logType.ToString(), logString);
            }
            else
            {
                // don't add log type to exceptions, as it's already in the string
                log.AppendLine(logString);
            }

            // enqueue the stack trace
            log.AppendLine(stackTrace);
        }
    }
}