using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public enum BodyType
    {
        Male = 0,
        Female = 1
    }

    public enum PartCategory
    {
        Armor = 0,
        Boots = 1,
        Ear = 2,
        Eyebrow = 3,
        Eyes = 4,
        FacialHair = 5,
        Gloves = 6,
        Hair = 7,
        Helmet = 8,
        Mouth = 9,
        Nose = 10,
        Pants = 11,
        SkinDetails = 12,
        Weapon = 13,
        Cape = 15,
        Skirt = 16,
        BodySkin = 17
    }

    public enum SlotCategory
    {
        Armor = 0,
        Boots = 1,
        Ear = 2,
        Eyebrow = 3,
        Eyes = 4,
        FacialHair = 5,
        Gloves = 6,
        Hair = 7,
        Helmet = 8,
        Mouth = 9,
        Nose = 10,
        Pants = 11,
        SkinDetails = 12,
        MainHand = 13,
        OffHand = 14,
        Cape = 15,
        Skirt = 16,
        BodySkin = 17
    }

    public enum WeaponCategory
    {
        OneHanded,
        TwoHanded,
        Bow,
        Shield,
        Gun,
        Rifle
    }

    public class ColorCode
    {
        /// <summary>
        /// First Color
        /// </summary>
        public const string Color1 = "_Color1";

        /// <summary>
        /// Second Color
        /// </summary>
        public const string Color2 = "_Color2";

        /// <summary>
        /// Third Color
        /// </summary>
        public const string Color3 = "_Color3";
    }

    public enum EmotionType
    {
        Blink = 10,
        Attack = 11,
        Hurt = 12,
        Talk = 13,

        // Custom Emote
        Emote_0 = 0,
        Emote_1 = 1,
        Emote_2 = 2,
        Emote_3 = 3,
        Emote_4 = 4,
        Emote_5 = 5,
        Emote_6 = 6,
        Emote_7 = 7,
        Emote_8 = 8,
        Emote_9 = 9
    }
}