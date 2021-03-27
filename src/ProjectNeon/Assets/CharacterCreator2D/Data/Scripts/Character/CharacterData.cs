using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public struct CharacterData
    {
        /// <summary>
        /// JSON's version of the data.
        /// </summary>
        public int dataVersion;

        /// <summary>
        /// Character's BodyType.
        /// </summary>
        public BodyType bodyType;

        /// <summary>
        /// Character's skin color.
        /// </summary>
        public Color skinColor;

        /// <summary>
        /// Character's tint color.
        /// </summary>
        public Color tintColor;

        /// <summary>
        /// List of PartSlotData
        /// </summary>
        public List<PartSlotData> slotData;

        public List<EmoteIndexData> emoteData;

        /// <summary>
        /// List of SegmentScaleData
        /// </summary>
        public List<SegmentScaleData> bodySegmentData;
    }

    [Serializable]
    public struct PartSlotData
    {
        /// <summary>
        /// Slot category.
        /// </summary>
        public SlotCategory category;

        /// <summary>
        /// Part name.
        /// </summary>
        public string partName;

        /// <summary>
        /// Package name.
        /// </summary>
		public string partPackage;

        /// <summary>
        /// First color.
        /// </summary>
        public Color color1;

        /// <summary>
        /// Second color.
        /// </summary>
        public Color color2;

        /// <summary>
        /// Third color.
        /// </summary>
        public Color color3;
    }

    [Serializable]
    public struct SegmentScaleData
    {
        /// <summary>
        /// Body segment's type
        /// </summary>
        public SegmentType segmentType;

        /// <summary>
        /// segment's scale
        /// </summary>
        public Vector2 scale;
    }

    [Serializable]
    public struct EmoteIndexData
    {
        public EmotionType emotionType;
        public string emotionName;
        public string eyebrowPartName;
        public string eyebrowPackage;
        public string eyesPartName;
        public string eyesPackage;
        public string nosePartName;
        public string nosePackage;
        public string mouthPartName;
        public string mouthPackage;
        public string earPartName;
        public string earPackage;
    }
}