using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FullscreenEditor {
    /// <summary>Toggle fullscreen upon playmode change if <see cref="FullscreenPreferences.FullscreenOnPlayEnabled"/> is set to true.</summary>
    [InitializeOnLoad]
    internal static class FullscreenOnPlay {

        static FullscreenOnPlay() {

            #if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += state => {
                switch (state) {
                    case PlayModeStateChange.ExitingEditMode:
                        SetIsPlaying(true);
                        break;

                    case PlayModeStateChange.ExitingPlayMode:
                        SetIsPlaying(false);
                        break;

                    case PlayModeStateChange.EnteredPlayMode:
                        foreach (var fs in Fullscreen.GetAllFullscreen())
                            if (fs && fs is FullscreenWindow && (fs as FullscreenWindow).CreatedByFullscreenOnPlay) {
                                FixGameViewMouseInput.UpdateGameViewArea(fs);
                            }
                        break;
                }
            };

            EditorApplication.pauseStateChanged += state => SetIsPlaying(EditorApplication.isPlayingOrWillChangePlaymode && state == PauseState.Unpaused);
            #else 
            EditorApplication.playmodeStateChanged += () => SetIsPlaying(EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPaused);
            #endif

        }

        private static void SetIsPlaying(bool playing) {

            var fullscreens = Fullscreen.GetAllFullscreen()
                .Select(fullscreen => fullscreen as FullscreenWindow)
                .Where(fullscreen => fullscreen);

            // We close all the game views created on play, even if the option was disabled in the middle of the play mode
            // This is done to best reproduce the default behaviour of the maximize on play
            if (!playing) {
                foreach (var fs in fullscreens)
                    if (fs && fs.CreatedByFullscreenOnPlay) // fs might have been destroyed
                        fs.Close();
                return;
            }

            if (!FullscreenPreferences.FullscreenOnPlayDeprecated && !FullscreenPreferences.FullscreenOnPlayEnabled)
                return; // Nothing to do here

            var gameView = FullscreenOnPlayGameView();

            if (!gameView) // no gameview has the fullscreen on play option enabled
                return;

            foreach (var fs in fullscreens)
                if (fs && fs.Rect.Overlaps(gameView.position)) // fs might have been destroyed
                    return; // We have an open fullscreen where the new one would be, so let it there

            if (gameView && Fullscreen.GetFullscreenFromView(gameView))
                return; // The gameview is already in fullscreen

            var gvfs = Fullscreen.MakeFullscreen(Types.GameView, gameView);
            gvfs.CreatedByFullscreenOnPlay = true;
        }

        internal static EditorWindow FullscreenOnPlayGameView() {
            return FullscreenPreferences.FullscreenOnPlayDeprecated ?
                FullscreenUtility
                .GetGameViews()
                .FirstOrDefault(gv => gv && GetEnterPlayModeBehavior(gv) == CustomEnterPlayModeBehavior.PlayFullscreen) :
                FullscreenUtility
                .GetGameViews()
                .FirstOrDefault(gv => gv && gv.GetPropertyValue<bool>("maximizeOnPlay"));
        }

        internal enum CustomEnterPlayModeBehavior {
            PlayFocused,
            PlayMaximized,
            PlayUnfocused,
            PlayFullscreen = 69534
        }

        internal static CustomEnterPlayModeBehavior GetEnterPlayModeBehavior(EditorWindow playmodeView) {
            return (CustomEnterPlayModeBehavior)playmodeView.GetPropertyValue<int>("enterPlayModeBehavior");
        }

        internal static void SetEnterPlayModeBehavior(EditorWindow playmodeView, CustomEnterPlayModeBehavior behaviour) {
            playmodeView.SetPropertyValue("enterPlayModeBehavior", (int)behaviour);
        }

        [InitializeOnLoadMethod]
        private static void AddFullscreenOptionToGameView() {
            if (!FullscreenPreferences.FullscreenOnPlayDeprecated)
                return;

            var newContent = new GUIContent("Fullscreen", FullscreenUtility.FullscreenOnPlayIcon, "Fullscreen on Play");
            var GetCachedEnumData = Types.EnumDataUtility.FindMethod("GetCachedEnumData", new [] {
                typeof(Type), // enumType
                typeof(bool), // excludeObsolete
                typeof(Func<string, string>) // nicifyName
            });

            var enumData = GetCachedEnumData.Invoke(null, new object[] {
                Types.EnterPlayModeBehavior,
                    true,
                    (Func<string, string>)ObjectNames.NicifyVariableName
            });

            // Retrieve fields from EnumData that need to be changed
            var values = enumData.GetFieldValue<Enum[]>("values");
            var flagValues = enumData.GetFieldValue<int[]>("flagValues");
            var displayNames = enumData.GetFieldValue<string[]>("displayNames");
            var tooltip = enumData.GetFieldValue<string[]>("tooltip");

            // Resize arrays to add one more element on each
            Array.Resize(ref values, values.Length + 1);
            Array.Resize(ref flagValues, flagValues.Length + 1);
            Array.Resize(ref displayNames, displayNames.Length + 1);
            Array.Resize(ref tooltip, tooltip.Length + 1);

            // Add custom value
            values[values.Length - 1] = (Enum)Enum.ToObject(Types.EnterPlayModeBehavior, (int)CustomEnterPlayModeBehavior.PlayFullscreen);
            flagValues[flagValues.Length - 1] = (int)CustomEnterPlayModeBehavior.PlayFullscreen;
            displayNames[displayNames.Length - 1] = newContent.text;
            tooltip[tooltip.Length - 1] = newContent.tooltip;

            // Reasign arrays since EnumData is a struct
            enumData.SetFieldValue("values", values);
            enumData.SetFieldValue("flagValues", flagValues);
            enumData.SetFieldValue("displayNames", displayNames);
            enumData.SetFieldValue("tooltip", tooltip);

            // Reasign EnumData instance on both dictionaries
            var s_EnumData = Types.EnumDataUtility.GetFieldValue<IDictionary>("s_EnumData"); // Dictionary<Type, EnumData>
            var s_NonObsoleteEnumData = Types.EnumDataUtility.GetFieldValue<IDictionary>("s_NonObsoleteEnumData"); // Dictionary<Type, EnumData>

            s_EnumData[Types.EnterPlayModeBehavior] = enumData;
            s_NonObsoleteEnumData[Types.EnterPlayModeBehavior] = enumData;

            // Update gameview dropdown when fullscreen on play changes
            FullscreenPreferences.FullscreenOnPlayEnabled.OnValueSaved += v => {
                var gv = FullscreenUtility.GetMainGameView();
                if (gv)
                    SetEnterPlayModeBehavior(gv, v ?
                        CustomEnterPlayModeBehavior.PlayFullscreen :
                        CustomEnterPlayModeBehavior.PlayFocused);
                EditorUtility.DisplayDialog("Fullscreen on play has changed", "Starting at Unity 2021.2.0 this option should be enabled on the game view window instead", "Got it!");
            };

            // update enum cache to add icon on GUIContent
            var EnumNamesCache = typeof(EditorGUI).GetNestedType("EnumNamesCache", BindingFlags.NonPublic);
            var s_EnumTypeLocalizedGUIContents = EnumNamesCache.GetFieldValue<Dictionary<Type, GUIContent[]>>("s_EnumTypeLocalizedGUIContents");

            // Invoke method to generate cache first
            EnumNamesCache.InvokeMethod("GetEnumTypeLocalizedGUIContents", Types.EnterPlayModeBehavior, s_EnumData[Types.EnterPlayModeBehavior]);

            // Update last one (which hopefully is fullscreen)
            var contents = s_EnumTypeLocalizedGUIContents[Types.EnterPlayModeBehavior];
            contents[contents.Length - 1] = newContent;

            EditorApplication.hierarchyWindowItemOnGUI += (a, b) => {
                // Update EditorStyles to add icon to the left
                EditorStyles.toolbarDropDown.imagePosition = ImagePosition.ImageLeft;
            };
        }

        [InitializeOnLoadMethod]
        private static void OverrideMaximizeOnPlay() {
            if (FullscreenPreferences.FullscreenOnPlayDeprecated)
                return;

            After.Frames(1, () => { // Call after one frame, so we don't acess the styles class before it's created

                var stylesClass = Types.GameView.GetNestedType("Styles", ReflectionUtility.FULL_BINDING);
                var currentContent = stylesClass.GetFieldValue<GUIContent>("maximizeOnPlayContent");

                var newContent = new GUIContent("Fullscreen on Play", FullscreenUtility.FullscreenOnPlayIcon);
                var originalContent = new GUIContent(currentContent);

                var overrideEnabled = FullscreenPreferences.FullscreenOnPlayEnabled;

                currentContent.text = overrideEnabled ? newContent.text : originalContent.text;
                currentContent.image = overrideEnabled ? newContent.image : originalContent.image;
                currentContent.tooltip = overrideEnabled ? newContent.tooltip : originalContent.tooltip;

                FullscreenPreferences.FullscreenOnPlayEnabled.OnValueSaved += v => {
                    currentContent.text = v ? newContent.text : originalContent.text;
                    currentContent.image = v ? newContent.image : originalContent.image;
                    currentContent.tooltip = v ? newContent.tooltip : originalContent.tooltip;

                    if (FullscreenUtility.GetMainGameView())
                        FullscreenUtility.GetMainGameView().SetPropertyValue("maximizeOnPlay", v);
                };

            });
        }

    }
}
