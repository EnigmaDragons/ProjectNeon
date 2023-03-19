using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace I2.Loc
{
	public partial class LocalizationEditor
	{
		#region Variables
		static string _Tools_NoLocalized_Include, 
 	   				  _Tools_NoLocalized_Exclude;
		const string _Help_Tool_NoLocalized = "This selects all labels in the current scene that don't have a Localized component.\n\nWhen Include or Exclude are set, labels will be filtered based on those settings.Separate by (,) if multiple strings are used.\n(e.g. Include:\"example,tutorial\")";
		#endregion
		
		#region GUI Find NoLocalized Terms

		[MenuItem("Neon/Localization/AllBuildSceneLocalizedProgress %#_l")]
		static void PrintNonLocalizedForEverySceneInBuild()
		{
			Log.Info("Begun Checking For Total Scene Localization Progress");
			var startingScene = SceneManager.GetActiveScene();
			var scenesInBuild = EditorBuildSettings.scenes;
			var length = scenesInBuild.Length;
			
			var finalDone = 0;
			var finalNeeded = 0;
			var allObjNamesNotLocalized = new List<string>();
			var csv = new List<string>();
			csv.Add("Scene, Progress, Done, Needed, Total");
			
			for (var i = 0; i < length; i++)
			{
				EditorSceneManager.OpenScene(scenesInBuild[i].path, OpenSceneMode.Single);
				var (done, objs) = GetNonLocalizedInCurrentScene();
				var total = objs.Count + done;
				var percentComplete = total == 0 ? 0 : (float)done / total;
				finalDone += done;
				finalNeeded += objs.Count;
				allObjNamesNotLocalized.AddRange(objs.Select(GetGameObjectSimplifiedName));
				Log.Info($"{SceneManager.GetActiveScene().name,-32} - Localization Progress: {percentComplete:P} - Done: {done} - Needed: {objs.Count}");
				csv.Add($"{SceneManager.GetActiveScene().name}, {percentComplete:P}, {done}, {objs.Count}, {total}");
			}
			LocalizationExporter.WriteCsv("SceneLocalizationProgress", csv);
			if (startingScene.IsValid()) 
				EditorSceneManager.OpenScene(startingScene.name);
			
			var finalTotal = finalDone + finalNeeded;
			var finalPercentComplete = finalTotal == 0 ? 0 : (float)finalDone / finalTotal;
			Log.Info($"Overall - Localization Progress: {finalPercentComplete:P} - Done: {finalDone} - Needed: {finalNeeded}");
			PrintTop5GameObjectNames(allObjNamesNotLocalized);
		}

		static string GetGameObjectSimplifiedName(GameObject o) =>
			Regex.Replace(o.gameObject.name, @"^((?:.{3})?[^_\s]*).*$", "$1");
		
		static void PrintTop5GameObjectNames(List<string> objNames)
		{
			objNames
				.GroupBy(a => a)
				.OrderByDescending(a => a.Count())
				.Take(5)
				.ForEach(a => Log.Info($"Object: {a.Key} - {a.Count()}"));
		}
		
		[MenuItem("Neon/Localization/SelectNonLocalized %&_l")]
		static void SelectNonLocalized()
		{
			EditorApplication.update += SelectNoLocalizedLabels;
		}
		
		void OnGUI_Tools_NoLocalized()
		{
			if (_Tools_NoLocalized_Include==null)
			{
				_Tools_NoLocalized_Include = EditorPrefs.GetString ("_Tools_NoLocalized_Include", string.Empty);
				_Tools_NoLocalized_Exclude = EditorPrefs.GetString ("_Tools_NoLocalized_Exclude", string.Empty);
			}
			
			GUILayout.Space (5);
			
			GUI.backgroundColor = Color.Lerp (Color.gray, Color.white, 0.2f);
			GUILayout.BeginVertical(LocalizeInspector.GUIStyle_OldTextArea, GUILayout.Height(1));
			GUI.backgroundColor = Color.white;
			
			EditorGUILayout.HelpBox(_Help_Tool_NoLocalized, MessageType.Info);

			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
				GUILayout.Label ("Include:", GUILayout.Width(60));
				_Tools_NoLocalized_Include = EditorGUILayout.TextArea(_Tools_NoLocalized_Include, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				GUILayout.Label ("Exclude:", GUILayout.Width(60));
				_Tools_NoLocalized_Exclude = EditorGUILayout.TextArea(_Tools_NoLocalized_Exclude, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();

			GUILayout.Space (5);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Select No Localized Labels"))
				EditorApplication.update += SelectNoLocalizedLabels;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndVertical();
		}
		
		#endregion
		
		
		#region Find No Localized

		static void SelectNoLocalizedLabels()
		{
			var (localizedCount, objs) = GetNonLocalizedInCurrentScene();
			if (objs.Count > 0)
			{
				Selection.objects = objs.ToArray();
				var percentComplete = (float)localizedCount / (objs.Count + localizedCount);
				Log.Info($"{SceneManager.GetActiveScene().name} - Localization Progress: {percentComplete:P} - Done: {localizedCount} - Needed: {objs.Count}");
				PrintTop5GameObjectNames(objs.Select(GetGameObjectSimplifiedName).ToList());
			}
			else
				ShowWarning("All labels in this scene have a Localize component assigned");
		}

		static (int done, List<GameObject> needed) GetNonLocalizedInCurrentScene()
		{
			EditorPrefs.SetString ("_Tools_NoLocalized_Include", _Tools_NoLocalized_Include);
			EditorPrefs.SetString ("_Tools_NoLocalized_Exclude", _Tools_NoLocalized_Exclude);

			EditorApplication.update -= SelectNoLocalizedLabels;

			List<Component> labels = new List<Component>();
			List<GameObject> Objs = new List<GameObject>();
			int localizedCount = 0;

			TextMesh[] textMeshes = (TextMesh[])Resources.FindObjectsOfTypeAll(typeof(TextMesh));
			if (textMeshes!=null && textMeshes.Length>0)
				labels.AddRange(textMeshes);

#if NGUI
			UILabel[] uiLabels = (UILabel[])Resources.FindObjectsOfTypeAll(typeof(UILabel));
			if (uiLabels!=null && uiLabels.Length>0)
				labels.AddRange(uiLabels);
#endif
			Text[] uiTexts = (Text[])Resources.FindObjectsOfTypeAll(typeof(Text));
			if (uiTexts!=null && uiTexts.Length>0)
				labels.AddRange(uiTexts);
#if TextMeshPro
			TextMeshPro[] tmpText = (TextMeshPro[])Resources.FindObjectsOfTypeAll(typeof(TextMeshPro));
			if (tmpText!=null && tmpText.Length>0)
				labels.AddRange(tmpText);

			TextMeshProUGUI[] uiTextsUGUI = (TextMeshProUGUI[])Resources.FindObjectsOfTypeAll(typeof(TextMeshProUGUI));
			if (uiTextsUGUI!=null && uiTextsUGUI.Length>0)
				labels.AddRange(uiTextsUGUI);
#endif
#if TK2D
			tk2dTextMesh[] tk2dTM = (tk2dTextMesh[])Resources.FindObjectsOfTypeAll(typeof(tk2dTextMesh));
			if (tk2dTM!=null && tk2dTM.Length>0)
				labels.AddRange(tk2dTM);
#endif

			if (labels.Count==0)
				return (localizedCount, Objs);

			string[] Includes = null;
			string[] Excludes = null;

			if (!string.IsNullOrEmpty (_Tools_NoLocalized_Include))
				Includes = _Tools_NoLocalized_Include.ToLower().Split(',', ';');

			if (!string.IsNullOrEmpty (_Tools_NoLocalized_Exclude))
				Excludes = _Tools_NoLocalized_Exclude.ToLower().Split(',', ';');

			for (int i=0, imax=labels.Count; i<imax; ++i)
			{
				Component label = labels[i];
				if (label==null || label.gameObject==null || !GUITools.ObjectExistInScene(label.gameObject))
					continue;

				if (labels[i].GetComponent<LocalizationNeeded>() == null && (
					    labels[i].GetComponent<Localize>() != null
				        || labels[i].GetComponent<NoLocalizationNeeded>() != null
				        || labels[i].GetComponent<UsesDynamicLocalization>() != null))
				{
					++localizedCount;
					continue;
				}

				if (ShouldFilter(label.name.ToLower(), Includes, Excludes))
					continue;

				Objs.Add( labels[i].gameObject );
			}

			return (localizedCount, Objs);
		}

		static bool ShouldFilter( string Text, string[] Includes, string[] Excludes )
		{
			if (Includes!=null && Includes.Length>0)
			{
				bool hasAny = false;
				for (int j=0; j<Includes.Length; ++j)
					if (Text.Contains(Includes[j]))
					{
						hasAny = true;
						break;
					}
				if (!hasAny)
					return true;
			}

			if (Excludes!=null && Excludes.Length>0)
			{
				for (int j=0; j<Excludes.Length; ++j)
					if (Text.Contains(Excludes[j]))
						return true;
			}

			return false;
		}

		#endregion
	}
}
