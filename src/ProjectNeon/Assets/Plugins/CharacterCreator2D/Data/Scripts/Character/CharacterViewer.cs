using CharacterCreator2D.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace CharacterCreator2D
{
    [HelpURL("http://bit.ly/CC2Ddoc")]
    public partial class CharacterViewer : MonoBehaviour
    {
        #region Public or Serialized Field

        [SerializeField]
        [FormerlySerializedAs("setupData")]
        private SetupData _setupData;

        /// <summary>
        /// CharacterViewer's body type.
        /// </summary>
        public BodyType bodyType;

        /// <summary>
        /// List of all PartSlots supported by CharacterViewer.
        /// </summary>
        public SlotList slots = new SlotList();

        /// <summary>
        /// List of all Emotes.
        /// </summary>
        public EmotionList emotes = new EmotionList();

        /// <summary>
        /// This value is used to change emotion in animation. make sure to set the animation curve to constant. -1 means no emotion/resetemote.
        /// </summary>
        public float emoteAnimIndex = -1;

        /// <summary>
        /// Should this CharacterViewer is initialized at Awake? You can turn it 'false' if you don't plan to change this CharacterViewer at runtime to save some process at Awake.
        /// </summary>
        public bool initializeAtAwake = true;

        /// <summary>
        /// Should the materials are instantiated at Runtime?
        /// </summary>
        public bool instanceMaterials = true;

        /// <summary>
        /// List of all sprites.
        /// </summary>
        public List<Renderer> sprites;

        /// <summary>
        /// List CharacterViewer's skins.
        /// </summary>
        public List<Renderer> skins;

        [SerializeField]
        private BodySegmentScaler _bodyscaler = new BodySegmentScaler();

        [SerializeField]
        private Color _skincolor = Color.gray;

        [SerializeField]
        private Color _tintcolor = Color.white;

        [SerializeField]
        [HideInInspector]
        private bool _isBaked;

        #endregion Public or Serialized Field

        #region Private Field

        private bool _isInstantiated;
        private bool _isMaterialInstantiated;
        private float _currentEmoteAnimIndex = -1;
        private EmoteIndex defaultEmote = new EmoteIndex();
        private bool isEmoting = false;
        private bool isEmotingAnimationEvent = false;

        private DataManager _dataManager = new DataManager();

        #endregion Private Field

        #region Properties

        /// <summary>
        /// CharacterViewer's skin color.
        /// </summary>
        public Color SkinColor
        {
            get => _skincolor;
            set
            {
                _skincolor = value;
                RepaintSkinColor();
            }
        }

        /// <summary>
        /// CharacterViewer's tint color. Default value is 'Color.white'.
        /// </summary>
        public Color TintColor
        {
            get => _tintcolor;
            set
            {
                _tintcolor = value;
                RepaintTintColor();
            }
        }

        /// <summary>
        /// CharacterViewer's setup data.
        /// </summary>
        public SetupData setupData
        {
            get
            {
                if (!_setupData)
                {
                    _setupData = SetupData.Static;
                }
                return _setupData;
            }
            set => _setupData = value; }

        public bool IsBaked => _isBaked;
        public bool IsInstantiated => _isInstantiated;
        public bool IsMaterialInstantiated => _isMaterialInstantiated;

        #endregion Properties

        #region MonoBehaviour

        private void Awake()
        {
            if (!_isBaked && instanceMaterials)
            {
                instantiateSlotsMaterial();
            }

            if (initializeAtAwake)
            {
                Initialize();
            }
            else if (!_isBaked)
            {
                getDefaultEmote();
            }
            _isInstantiated = true;
        }

        private void Update()
        {
            // check emotion change in animation and update emotion if necessary
            if (!_isBaked && !isEmoting && emoteAnimIndex != _currentEmoteAnimIndex)
            {
                updateAnimEmote(emoteAnimIndex);
            }
        }

        #endregion MonoBehaviour

        #region Initialization

        /// <summary>
        /// Initialize CharacterViewer according to the corresponded parts and settings.
        /// </summary>
        public void Initialize()
        {
            if (_isBaked)
            {
                return;
            }
            refreshBodyType();
            sprites = getSprites(setupData.order);
            skins = getSprites(setupData.skin);
            sortSprites();
            refreshSortingGroup();
            RelinkMaterials();
            RepaintSkinColor();
            RepaintTintColor();
            refreshPartSlots();
            emotes.resetPresetName();
            getDefaultEmote();
        }

        private void instantiateSlotsMaterial()
        {
            if (_isMaterialInstantiated)
            {
                return;
            }
            Dictionary<Material, Material> materials = new Dictionary<Material, Material>();
            foreach (SlotCategory cat in CharacterUtility.slotCategories)
            {
                PartSlot slot = slots.GetSlot(cat);
                if (slot.material == null)
                {
                    continue;
                }
                if (!materials.TryGetValue(slot.material, out Material tmat))
                {
                    tmat = Instantiate(slot.material);
                    tmat.name = slot.material.name;
                    materials.Add(slot.material, tmat);
                    slot.material = tmat;
                }
                else
                {
                    slot.material = tmat;
                }
            }
            _isMaterialInstantiated = true;
        }

        /// <summary>
        /// Relink all materials used by this CharacterViewer to the value in each of its slots.
        /// </summary>
        public void RelinkMaterials()
        {
            var assigner = setupData.GetCharacterManager(bodyType).Assigner;
            foreach (SlotCategory s in CharacterUtility.slotCategories)
            {
                assigner.AssignMaterialToRenderer(this, s);
            }
        }

        private List<Renderer> getSprites(List<string> nameList)
        {
            Renderer[] allSprites = GetComponentsInChildren<Renderer>(true);
            List<Renderer> outputSprites = new List<Renderer>();
            for (int n = 0; n < nameList.Count; n++)
            {
                for (int s = 0; s < allSprites.Length; s++)
                {
                    if (allSprites[s].name == nameList[n])
                    {
                        outputSprites.Add(allSprites[s]);
                        break;
                    }
                }
            }
            return outputSprites;
        }

        private void sortSprites()
        {
            for (int a = 0; a < setupData.order.Count; a++)
            {
                for (int s = 0; s < sprites.Count; s++)
                {
                    if (sprites[s].name == setupData.order[a])
                    {
                        sprites[s].sortingOrder = a;
                        break;
                    }
                }
            }
        }

        private void refreshSortingGroup()
        {
            SortingGroup sortinggroup = GetComponent<SortingGroup>();
            sortinggroup.enabled = false;
            sortinggroup.enabled = true;
        }

        private void refreshPartSlots()
        {
            var manager = setupData.GetCharacterManager(bodyType);
            var setter = manager.Setter;
            var assigner = manager.Assigner;
            foreach (SlotCategory cat in CharacterUtility.slotCategories)
            {
                PartSlot partSlot = slots[cat];
                setter.SetPartToSlot(this, cat, partSlot.assignedPart);
                assigner.AssignPartToRenderer(this, cat);
                assigner.AssignColorToRenderer(this, cat);
            }
        }

        #endregion Initialization

        #region BodyType

        /// <summary>
        /// Change this CharacterViewer's BodyType.
        /// </summary>
        /// <param name="bodyType">Desired BodyType value.</param>
        public void SetBodyType(BodyType bodyType)
        {
            this.bodyType = bodyType;
            Initialize();
        }

        private void refreshBodyType()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            Transform bodyobj = setupData.GetBodyPrefab(bodyType);
            if (bodyobj == null)
            {
                return;
            }

            detachEntities();
            Animator animator = GetComponent<Animator>();
            int curranimBase = 0;
            int curranimAim = 0;
            float currAim = 0f;
            if (Application.isPlaying && animator.GetCurrentAnimatorClipInfo(0).Length > 0)
            {
                curranimBase = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
                curranimAim = animator.GetCurrentAnimatorStateInfo(1).fullPathHash;
                currAim = animator.GetFloat("Aim");
            }
            animator.Play("Default", 0);
            animator.Play("None", 1);
            Transform oldbody = transform.Find("Root");
            if (oldbody != null)
            {
                DestroyImmediate(oldbody.gameObject);
            }

            bodyobj = Instantiate(bodyobj, transform.position, transform.rotation, transform);
            bodyobj.name = "Root";
            animator.Rebind();
            animator.Play(curranimBase, 0);
            animator.Play(curranimAim, 1);
            animator.SetFloat("Aim", currAim);
            _bodyscaler.Initialize(this);
            attachEntities();
        }

        #endregion BodyType

        #region Equipment

        /// <summary>
        /// Returns the assigned part of a given SlotCategory. Returns 'null' if there is no part assigned.
        /// </summary>
        /// <param name="category">Given SlotCategory.</param>
        /// <returns>Assigned part of the given SlotCategory if there is any, otherwise returns 'null'.</returns>
        public Part GetAssignedPart(SlotCategory category)
        {
            return slots[category].assignedPart;
        }

        /// <summary>
        /// Assign/unassign part to/from desired slot of this CharacterViewer.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="partName">The name of the part. It will assign the first part found if there are more than one part with the same name in different packages. Will unassign if 'null' or 'empty'.</param>
        public void EquipPart(SlotCategory slotCategory, string partName)
        {
            EquipPart(slotCategory, partName, "");
        }

        /// <summary>
        /// Assign/unassign part to/from desired slot of this CharacterViewer.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="partName">The name of the part. Will unassign if 'null' or 'empty'.</param>
        /// <param name="partPackage">The package name of the part. Will assign the first part found if 'null' or 'empty'</param>
        public void EquipPart(SlotCategory slotCategory, string partName, string partPackage)
        {
            if (string.IsNullOrEmpty(partName))
            {
                EquipPart(slotCategory, (Part)null);
                return;
            }

            Part part = PartList.Static.FindPart(partName, partPackage, slotCategory);
            if (part == null)
            {
                Debug.Log("can't find part: " + partName);
                return;
            }

            EquipPart(slotCategory, part);
        }

        /// <summary>
        /// Assign/unassign part to/from desired slot of this CharacterViewer.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="part">Desired Part. Will unassigned if 'null'.</param>
        public void EquipPart(SlotCategory slotCategory, Part part)
        {
            var manager = setupData.GetCharacterManager(bodyType);
            manager.Setter.SetPartToSlot(this, slotCategory, part);
            if (!_isBaked)
            {
                if (Utilities.Humanoid.HumanoidUtility.IsFace(slotCategory))
                {
                    if (!isEmoting && !isEmotingAnimationEvent)
                    {
                        getDefaultEmote();
                    }
                }
            }
            manager.Assigner.AssignPartToRenderer(this, slotCategory);
        }

        #endregion Equipment

        #region Colorization

        /// <summary>
        /// Repaint skin color according to the current value.
        /// </summary>
        public void RepaintSkinColor()
        {
            setupData.GetCharacterManager(bodyType).Assigner.AssignSkinColorToRenderer(this);
        }

        /// <summary>
        /// Repaint tint color according to the current value.
        /// </summary>
        public void RepaintTintColor()
        {
            setupData.GetCharacterManager(bodyType).Assigner.AssignTintColorToRenderer(this);
        }

        /// <summary>
        /// Returns Part's Color from desired PartSlot.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <returns>Part's Color. Returns 'Color.clear' if colorCode doesn't match with any value in 'CharacterCreator2D.ColorCode'.</returns>
        public Color GetPartColor(SlotCategory slotCategory, string colorCode)
        {
            switch (colorCode)
            {
                case ColorCode.Color1:
                    return slots[slotCategory].color1;
                case ColorCode.Color2:
                    return slots[slotCategory].color2;
                case ColorCode.Color3:
                    return slots[slotCategory].color3;
                default:
                    return Color.clear;
            }
        }

        /// <summary>
        /// Modifies Part's Color.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <param name="color">Desired value of the color.</param>
        public void SetPartColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            var manager = setupData.GetCharacterManager(bodyType);
            manager.Setter.SetColorToSlot(this, slotCategory, colorCode, color);
            manager.Assigner.AssignColorToRenderer(this, slotCategory);
        }

        /// <summary>
        /// Modifies Part's Color.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <param name="color1">color for the 1st color slot of the equipment</param>
        /// <param name="color2">color for the 2nd color slot of the equipment</param>
        /// <param name="color3">color for the 3rd color slot of the equipment</param>
        public void SetPartColor(SlotCategory slotCategory, Color color1, Color color2, Color color3)
        {
            SetPartColor(slotCategory, ColorCode.Color1, color1);
            SetPartColor(slotCategory, ColorCode.Color2, color2);
            SetPartColor(slotCategory, ColorCode.Color3, color3);
        }

        #endregion Colorization

        #region BodySlider

        /// <summary>
        /// Modifies body slider's value of this character.
        /// </summary>
        /// <param name="segmentType">Selected segment.</param>
        /// <param name="scale">Desired scale ranged from 0 to 1. Default value is 0.5f. Different values will be forced to equals if the selected segment is symmetrical.</param>
        public void SetBodySlider(SegmentType segmentType, Vector2 scale)
        {
            _bodyscaler.SetScale(segmentType, scale);
        }

        /// <summary>
        /// Get selected body slider's value of this character.
        /// </summary>
        /// <param name="segmentType">Selected segment.</param>
        /// <returns>Selected body slider's value.</returns>
        public Vector2 GetBodySlider(SegmentType segmentType)
        {
            return _bodyscaler.GetScale(segmentType);
        }

        #endregion BodySlider

        #region Bake

        public void Bake()
        {
            if (!Application.isPlaying)
            {
                throw new Exception("Bake is for Runtime only");
            }
            if (!_isInstantiated)
            {
                throw new Exception("Cannot bake prefab or un-instantiated object");
            }
            var manager = setupData.GetCharacterManager(bodyType);
            BakedSlotsToSlots();
            // init bodytype
            refreshBodyType();
            sprites = getSprites(setupData.order);
            skins = getSprites(setupData.skin);
            sortSprites();
            refreshSortingGroup();
            foreach (var cat in CharacterUtility.slotCategories)
            {
                manager.Setter.SetPartToSlot(this, cat, slots[cat].assignedPart);
            }
            manager.Baker.BakeSlots(this);
            manager.Assigner.AssignTintColorToRenderer(this);
            SlotsToBakedSlots();
            Resources.UnloadUnusedAssets();
            _isBaked = true;
        }

        public void Unbake()
        {
            if (!Application.isPlaying)
            {
                throw new Exception("Bake is for Runtime only");
            }
            if (!_isInstantiated)
            {
                throw new Exception("Cannot bake prefab or un-instantiated object");
            }
            if (_isBaked)
            {
                _isBaked = false;
                BakedSlotsToSlots();
                instantiateSlotsMaterial();
                Initialize();
                Resources.UnloadUnusedAssets();
            }
        }

        private void SlotsToBakedSlots()
        {
            foreach (var cat in CharacterUtility.slotCategories)
            {
                var partSlot = slots[cat];
                partSlot.assignedPart = null;
                if (partSlot.material)
                {
                    var propIds = partSlot.material.GetTexturePropertyNameIDs();
                    for (int i = 0; i < propIds.Length; i++)
                    {
                        partSlot.material.SetTexture(propIds[i], null);
                    }
                }
            }
        }

        private void BakedSlotsToSlots()
        {
            foreach (var cat in CharacterUtility.slotCategories)
            {
                var partSlot = slots[cat];
                if (partSlot.assignedPart)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(partSlot.AssignedPartName))
                {
                    partSlot.assignedPart = PartList.Static.FindPart(partSlot.AssignedPartName, partSlot.AssignedPackageName, cat);
                }
                else
                {
                    partSlot.assignedPart = null;
                }
            }
        }

        #endregion Bake

        #region JSON

        /// <summary>
        /// Generate CharacterData of CharacterViewer.
        /// </summary>
        /// <returns>Generated CharacterData of CharacterViewer.</returns>
        public CharacterData GenerateCharacterData()
        {
            return _dataManager.CreateCharacterData(this);
        }

        /// <summary>
        /// Assign and initialize this CharacterViewer according to a given CharacterData.
        /// </summary>
        /// <param name="data">CharacterData to be assigned from.</param>
        public void AssignCharacterData(CharacterData data)
        {
            _dataManager.AssignCharacterData(this, data);
        }

        /// <summary>
        /// Save CharacterViewer's data as JSON file on a given path.
        /// </summary>
        /// <param name="filePath">Desired file path.</param>
        /// <returns>Returns 'true' on success, otherwise 'false'.</returns>
        public bool SaveToJSON(string filePath)
        {
            try
            {
                CharacterData data = GenerateCharacterData();
                string content = JsonUtility.ToJson(data, true);
                string directory = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Directory.GetCurrentDirectory();
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("error on save to JSON:\n" + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Load and assign data from a JSON file in a given path to CharacterViewer.
        /// </summary>
        /// <param name="filePath">Desired file path.</param>
        /// <returns>Returns 'true' on success, otherwise 'false'.</returns>
        public bool LoadFromJSON(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("load CharacterViewer '" + name + "' from JSON's failed: file doesn't exist");
                return false;
            }

            try
            {
                CharacterData data = JsonUtility.FromJson<CharacterData>(File.ReadAllText(filePath));
                AssignCharacterData(data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("error on load from JSON:\n" + e.ToString());
                return false;
            }
        }

        #endregion JSON

        #region character entities

        private List<CharacterEntity> _entities;

        private void detachEntities()
        {
            _entities = new List<CharacterEntity>();
            transform.GetComponentsInChildren<CharacterEntity>(true, _entities);
            foreach (CharacterEntity e in _entities)
            {
                e.Detach(this);
            }
        }

        private void attachEntities()
        {
            if (_entities == null || _entities.Count <= 0)
            {
                return;
            }
            foreach (CharacterEntity e in _entities)
            {
                e.Attach(this);
            }
        }

        #endregion character entities
    }
}
