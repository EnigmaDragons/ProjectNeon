namespace AeLa.EasyFeedback.Editor
{
	internal static class Constants
	{
		public const string ASSET_ROOT_DIRECTORY = "Easy Feedback";
		public const string CONFIG_ASSET_NAME = "EasyFeedbackConfig.asset";
		public const string PROJECT_SETTINGS_PATH = "Project/Easy Feedback";

		public const string FILESIZE_WARNING =
			"Trello has a per-attachment file size limit of 10MB for free accounts and 250MB for paid accounts. Files larger than this limit will fail to upload.";

        /// <summary>
        /// Forward slash (/) as unicode sequence. 
        /// Used as a workaround for forward slashes in Trello board names
        /// causing popups to erronously display submenus.
        /// </summary>
        public const string UNICODE_FORWARD_SLASH = "\u2215";
	}
}