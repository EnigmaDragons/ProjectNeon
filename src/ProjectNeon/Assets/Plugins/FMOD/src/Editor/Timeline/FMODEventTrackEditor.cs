#if UNITY_TIMELINE_EXIST && UNITY_2019_2_OR_NEWER

using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace FMODUnity
{
    [CustomTimelineEditor(typeof(FMODEventTrack))]
    public class FMODEventTrackEditor : TrackEditor
    {
        static readonly Texture2D icon = EditorGUIUtility.Load("FMOD/StudioIcon.png") as Texture2D;

        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            TrackDrawOptions options = base.GetTrackOptions(track, binding);
            options.icon = icon;

            return options;
        }
    }
}

#endif
