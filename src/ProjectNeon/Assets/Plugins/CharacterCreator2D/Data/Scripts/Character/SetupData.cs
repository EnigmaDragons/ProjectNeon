using CharacterCreator2D.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class SetupData : ScriptableObject
    {
        #region const

        /// <summary>
        /// partLinks['slot_category']['sprite_name'] = 'transform_hierarchy'
        /// </summary>
        public static readonly Dictionary<SlotCategory, Dictionary<string, string>> partLinks = new Dictionary<SlotCategory, Dictionary<string, string>>()
        {
            {
                SlotCategory.Armor,
                new Dictionary<string, string>()
                {
                    { "Armor Body","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Body/Armor Body" },
                    { "Armor Lower Arm L","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Lower Arm L/Armor Lower Arm L" },
                    { "Armor Lower Arm R","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Lower Arm R/Armor Lower Arm R" },
                    { "Armor Neck","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Neck/Armor Neck" },
                    { "Armor Upper Arm L","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Upper Arm L/Armor Upper Arm L" },
                    { "Armor Upper Arm R","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Upper Arm R/Armor Upper Arm R" }
                }
            },
            {
                SlotCategory.Boots,
                new Dictionary<string, string>()
                {
                    { "Boots Foot F","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Pos_Foot R/Bone_Foot R/Foot F/Boots Foot F" },
                    { "Boots Foot L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Pos_Lower Leg L/Bone_Lower Leg L/Pos_Foot L/Bone_Foot L/Foot L/Boots Foot L" },
                    { "Boots Foot R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Pos_Foot R/Bone_Foot R/Foot R/Boots Foot R" },
                    { "Boots Lower Leg F","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Lower Leg F/Boots Lower Leg F" },
                    { "Boots Lower Leg L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Pos_Lower Leg L/Bone_Lower Leg L/Lower Leg L/Boots Lower Leg L" },
                    { "Boots Lower Leg R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Lower Leg R/Boots Lower Leg R" }
                }
            },
            {
                SlotCategory.Ear,
                new Dictionary<string, string>()
                {
                    { "Ear R","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Ear R" },
                    { "Ear L","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Ear L" }
                }
            },
            {
                SlotCategory.Eyebrow,
                new Dictionary<string, string>()
                {
                    { "Eyebrow","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Eyebrow" }
                }
            },
            {
                SlotCategory.Eyes,
                new Dictionary<string, string>()
                {
                    { "Eyes B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Eyes B" },
                    { "Eyes F","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Eyes F" }
                }
            },
            {
                SlotCategory.FacialHair,
                new Dictionary<string, string>()
                {
                    { "Facial Hair","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Facial Hair" }
                }
            },
            {
                SlotCategory.Gloves,
                new Dictionary<string, string>()
                {
                    { "Gloves Hand L 0","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Hand L 0/Gloves Hand L 0" },
                    { "Gloves Hand L 1","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Hand L 1/Gloves Hand L 1" },
                    { "Gloves Hand R 0","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Pos_Hand R/Bone_Hand R/Hand R 0/Gloves Hand R 0" },
                    { "Gloves Hand R 1","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Pos_Hand R/Bone_Hand R/Hand R 1/Gloves Hand R 1" },
                    { "Gloves Lower Arm L","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Lower Arm L/Gloves Lower Arm L" },
                    { "Gloves Lower Arm R","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Lower Arm R/Gloves Lower Arm R" }
                }
            },
            {
                SlotCategory.Hair,
                new Dictionary<string, string>()
                {
                    { "Hair B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Hair B" },
                    { "Hair F","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Hair F" },
                    { "Hair S","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Hair S" }
                }
            },
            {
                SlotCategory.Helmet,
                new Dictionary<string, string>()
                {
                    { "Helmet B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Helmet B" },
                    { "Helmet F","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Helmet F" }
                }
            },
            {
                SlotCategory.Mouth,
                new Dictionary<string, string>()
                {
                    { "Mouth B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Mouth B" },
                    { "Mouth F","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Mouth F" }
                }
            },
            {
                SlotCategory.Nose,
                new Dictionary<string, string>()
                {
                    { "Nose","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Nose" }
                }
            },
            {
                SlotCategory.Pants,
                new Dictionary<string, string>()
                {
                    { "Pants Hip","Root/Pos_Hip/Bone_Hip/Hip/Pants Hip" },
                    { "Pants Lower Leg F","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Lower Leg F/Pants Lower Leg F" },
                    { "Pants Lower Leg L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Pos_Lower Leg L/Bone_Lower Leg L/Lower Leg L/Pants Lower Leg L" },
                    { "Pants Lower Leg R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Lower Leg R/Pants Lower Leg R" },
                    { "Pants Upper Leg L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Upper Leg L/Pants Upper Leg L" },
                    { "Pants Upper Leg R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Upper Leg R/Pants Upper Leg R" }
                }
            },
            {
                SlotCategory.BodySkin,
                new Dictionary<string, string>()
                {
                    { "Body","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Body" },
                    { "Foot F","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Pos_Foot R/Bone_Foot R/Foot F" },
                    { "Foot L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Pos_Lower Leg L/Bone_Lower Leg L/Pos_Foot L/Bone_Foot L/Foot L" },
                    { "Foot R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Pos_Foot R/Bone_Foot R/Foot R" },
                    { "Hand L 0","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Hand L 0" },
                    { "Hand L 1","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Hand L 1" },
                    { "Hand R 0","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Pos_Hand R/Bone_Hand R/Hand R 0" },
                    { "Hand R 1","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Pos_Hand R/Bone_Hand R/Hand R 1" },
                    { "Head","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head" },
                    { "Hip","Root/Pos_Hip/Bone_Hip/Hip" },
                    { "Lower Arm L","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Lower Arm L" },
                    { "Lower Arm R","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Lower Arm R" },
                    { "Lower Leg F","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Lower Leg F" },
                    { "Lower Leg L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Pos_Lower Leg L/Bone_Lower Leg L/Lower Leg L" },
                    { "Lower Leg R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Pos_Lower Leg R/Bone_Lower Leg R/Lower Leg R" },
                    { "Upper Arm L","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Upper Arm L" },
                    { "Upper Arm R","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Upper Arm R" },
                    { "Upper Leg L","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg L/Bone_Upper Leg L/Upper Leg L" },
                    { "Upper Leg R","Root/Pos_Hip/Bone_Hip/Pos_Upper Leg R/Bone_Upper Leg R/Upper Leg R" },
                    { "Neck","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Neck" }
                }
            }
        };

        public static readonly Dictionary<string, string> lWeaponLink = new Dictionary<string, string>()
        {
            { "Weapon","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Weapon L" }
        };

        public static readonly Dictionary<string, string> rWeaponLink = new Dictionary<string, string>()
        {
            { "Weapon","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Pos_Hand R/Bone_Hand R/Pos_Weapon R/Bone_Weapon R/Weapon R" }
        };

        public static readonly Dictionary<string, string> bowLink = new Dictionary<string, string>()
        {
            { "Bow B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Bow B" },
            { "Bow T","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Bow T" },
            { "Bowstring B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Bow B/Bowstring B" },
            { "Bowstring S","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Bowstring S" },
            { "Bowstring T","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Bow T/Bowstring T" }
        };

        public static readonly Dictionary<string, string> shieldLink = new Dictionary<string, string>()
        {
            { "Shield B","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Shield B" },
            { "Shield F","Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Shield F" }
        };

        public static readonly Dictionary<SlotCategory, List<string>> colorableSpriteLinks = new Dictionary<SlotCategory, List<string>>()
        {
            {
                SlotCategory.Eyebrow,
                new List<string>()
                {
                    "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Eyebrow"
                }
            },
            {
                SlotCategory.Eyes,
                new List<string>()
                {
                    "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Eyes F"
                }
            },
            {
                SlotCategory.FacialHair,
                new List<string>()
                {
                    "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Facial Hair"
                }
            },
            {
                SlotCategory.Hair,
                new List<string>()
                {
                     "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Hair B" ,
                     "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Hair F" ,
                     "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Hair S"
                }
            },
            {
                SlotCategory.Mouth,
                new List<string>()
                {
                    "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Neck/Bone_Neck/Pos_Head/Bone_Head/Head/Mouth F"
                }
            }
        };

        public static readonly Dictionary<SlotCategory, string> weaponFXLinks = new Dictionary<SlotCategory, string>()
        {
            { SlotCategory.MainHand, "Root/FX/Weapon Slash R" },
            { SlotCategory.OffHand, "Root/FX/Weapon Slash L" }
        };

        public static readonly Dictionary<SlotCategory, string> muzzleFXLinks = new Dictionary<SlotCategory, string>()
        {
            { SlotCategory.MainHand, "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm R/Bone_Upper Arm R/Pos_Lower Arm R/Bone_Lower Arm R/Pos_Hand R/Bone_Hand R/Pos_Weapon R/Bone_Weapon R/Muzzle R" },
            { SlotCategory.OffHand, "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Muzzle L" }
        };

        public const string arrowLink = "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Pos_Upper Arm L/Bone_Upper Arm L/Pos_Lower Arm L/Bone_Lower Arm L/Pos_Hand L/Bone_Hand L/Pos_Weapon L/Bone_Weapon L/Arrow";

        public const string capeLink = "Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Cape/Mesh_Cape";

        public const string skirtLink = "Root/Pos_Hip/Bone_Hip/Skirt/Mesh_Skirt";

        #endregion const

        public int dataVersion;
        public List<string> order;
        public List<string> skin;
        public List<BodyTypeData> bodyTypeData;
        public List<MaterialData> defaultMaterials;
        public Material defaultBakedMaterial;

        private bool m_isBodyTypeDataLoaded;
        private readonly Dictionary<BodyType, BodyTypeData> m_bodyTypeData = new Dictionary<BodyType, BodyTypeData>();

        private static SetupData m_setupData;

        public static SetupData Static
        {
            get
            {
                if (!m_setupData)
                {
                    m_setupData = Resources.Load<SetupData>("CC2D_SetupData");
                }
                return m_setupData;
            }
        }

        /// <summary>
        /// Returns base object of a given BodyType.
        /// </summary>
        /// <param name="bodyType"></param>
        /// <returns></returns>
        public Transform GetBodyPrefab(BodyType bodyType)
        {
            foreach (BodyTypeData d in bodyTypeData)
            {
                if (d.bodyType == bodyType)
                    return d.prefab;
            }
            return null;
        }

        public CharacterManager GetCharacterManager(BodyType bodyType)
        {
            LoadBodyTypeData();
            if (!m_bodyTypeData.TryGetValue(bodyType, out BodyTypeData result))
            {
                return CharacterManager.DefaultManager;
            }
            if (!result.manager)
            {
                return CharacterManager.DefaultManager;
            }
            return result.manager;
        }

        private void LoadBodyTypeData()
        {
            if (!m_isBodyTypeDataLoaded)
            {
                foreach (var btd in bodyTypeData)
                {
                    if (btd == null)
                    {
                        continue;
                    }
                    m_bodyTypeData[btd.bodyType] = btd;
                }
                m_isBodyTypeDataLoaded = true;
            }
        }

        private void OnDisable()
        {
            m_isBodyTypeDataLoaded = false;
            m_bodyTypeData.Clear();
        }
    }
}
