#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
using UnityEngine;
using MoreMountains.Feedbacks;
using Lofelt.NiceVibrations;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Use this feedback to play a preset haptic, limited but super simple predifined haptic patterns
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Haptics/Haptic Preset")]
    [FeedbackHelp("Use this feedback to play a preset haptic, limited but super simple predifined haptic patterns")]
    public class MMFeedbackNVPreset : MMFeedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        #if UNITY_EDITOR
            public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.HapticsColor; } }
        #endif
    
        [Header("Haptic Preset")]
        /// the preset to play with this feedback
        [Tooltip("the preset to play with this feedback")]
        public HapticPatterns.PresetType Preset = HapticPatterns.PresetType.LightImpact;

        [Header("Settings")] 
        /// a set of settings you can tweak to specify how and when exactly this haptic should play
        [Tooltip("a set of settings you can tweak to specify how and when exactly this haptic should play")]
        public MMFeedbackNVSettings HapticSettings;
        
        /// <summary>
        /// On play we play our preset haptic
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized || !HapticSettings.CanPlay())
            {
                return;
            }

            HapticSettings.SetGamepad();
            HapticPatterns.PlayPreset(Preset);
        }
    }    
}
#endif