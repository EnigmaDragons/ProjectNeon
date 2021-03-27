using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CharacterCreator2D;

namespace CharacterEditor2D
{
    public static class WizardUtils
    {
        public const string PartTemplateFolder = "Assets/CharacterCreator2D/Data/Part Templates";
        public const string PaletteFolder = "Assets/CharacterCreator2D/Color Palettes";
        public const string PartFolder = "Assets/CharacterCreator2D/Parts";
        public const string SetupDataPath = "Assets/CharacterCreator2D/Data/Resources/CC2D_SetupData.asset";
        public const string PartListPath = "Assets/CharacterCreator2D/Data/Resources/CC2D_PartList.asset";

        private static GUIStyle _bgstyle = createBGStyle();
        public static GUIStyle BGStyle
        {
            get { return _bgstyle; }
        }

        private static GUIStyle createBGStyle()
        {
            GUIStyle val = new GUIStyle();
            val.normal.background = EditorUtils.CreateTexture(1, 1, new Color(0.6f, 0.6f, 0.6f, 1.0f));
            return val;
        }

        private static GUIStyle _boldtxtstyle = createBoldTextStyle();
        public static GUIStyle BoldTextStyle
        {
            get { return _boldtxtstyle; }
        }

        private static GUIStyle createBoldTextStyle()
        {
            GUIStyle val = new GUIStyle();
            val.fontStyle = FontStyle.Bold;
            return val;
        }

        public static string GetDirectoryName(PartCategory partCategory)
        {
            switch (partCategory)
            {
                case PartCategory.FacialHair:
                    return "Facial Hair";
                case PartCategory.SkinDetails:
                    return "Skin Details";
                case PartCategory.BodySkin:
                    return "Body Skin";
                default:
                    return "" + partCategory;
            }
        }

        public static string GetDirectoryName(WeaponCategory weaponCategory)
        {
            switch (weaponCategory)
            {
                case WeaponCategory.OneHanded:
                    return "One Handed";
                case WeaponCategory.TwoHanded:
                    return "Two Handed";
                default:
                    return "" + weaponCategory;
            }
        }

        public static string RelativePartPath(Part part)
        {
            if (part)
            {
                if (part is Weapon weapon)
                {
                    return part.packageName + "/Weapon/" + GetDirectoryName(weapon.weaponCategory) + "/" + part.name + ".asset";
                }
                else
                {
                    return part.packageName + "/" + GetDirectoryName(part.category) + "/" + part.name + ".asset";
                }
            }
            return "";
        }
    }
}
