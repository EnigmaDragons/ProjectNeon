using UnityEditor;
using UnityEngine;

namespace AeLa.EasyFeedback.Editor
{
	[CustomEditor(typeof(FeedbackForm))]
	internal class FeedbackFormEditor : UnityEditor.Editor
	{
		private SerializedProperty config;

		void Awake()
		{
			config = serializedObject.FindProperty("Config");
		}

		public override void OnInspectorGUI()
		{
			// prompt to configure if not already set up
			if (config == null || !config.objectReferenceValue)
			{
				EditorGUILayout.LabelField("Easy Feedback is not yet configured!");
				if (GUILayout.Button("Configure Now"))
				{
					SettingsService.OpenProjectSettings(Constants.PROJECT_SETTINGS_PATH);
				}
			}
			else
			{
				EFConfig config = this.config.objectReferenceValue as EFConfig;
				if (string.IsNullOrEmpty(config.Token))
				{
					EditorGUILayout.LabelField("Not authenticated with Trello!");
					if (GUILayout.Button("Authenticate Now"))
					{
						SettingsService.OpenProjectSettings(Constants.PROJECT_SETTINGS_PATH);
					}
				}
				else
				{
					// show filesize warning
					EditorGUILayout.HelpBox(Constants.FILESIZE_WARNING, MessageType.Warning);
				}
			}

			base.OnInspectorGUI();
		}
	}
}