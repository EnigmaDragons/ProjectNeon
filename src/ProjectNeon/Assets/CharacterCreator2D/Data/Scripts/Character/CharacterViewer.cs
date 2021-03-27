using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace CharacterCreator2D
{
    [HelpURL("http://bit.ly/CC2Ddoc")]
    public class CharacterViewer : MonoBehaviour
    {
        /// <summary>
        /// CharacterViewer's setup data.
        /// </summary>
        public SetupData setupData;

        /// <summary>
        /// CharacterViewer's body type.
        /// </summary>
        public BodyType bodyType;

        /// <summary>
        /// List of all PartSlots supported by CharacterViewer.
        /// </summary>
        public SlotList slots;

        /// <summary>
        /// List of all Emotes.
        /// </summary>
        public EmotionList emotes;

        /// <summary>
        /// This value is used to change emotion in animation. make sure to set the animation curve to constant. -1 means no emotion/resetemote.
        /// </summary>
        public float emoteAnimIndex = -1;

        private float _currentEmoteAnimIndex = -1;

        private EmoteIndex defaultEmote = new EmoteIndex();
        private bool isEmoting = false;
        private bool isEmotingAnimationEvent = false;

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
        private BodySegmentScaler _bodyscaler = null;
        [SerializeField]
        private Color _skincolor = Color.gray;
        [SerializeField]
        private Color _tintcolor = Color.white;

        /// <summary>
        /// CharacterViewer's skin color.
        /// </summary>
        public Color SkinColor
        {
            get { return _skincolor; }
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
            get { return _tintcolor; }
            set 
            { 
                _tintcolor = value; 
                RepaintTintColor();
            }
        }

        void Awake()
        {
            if (instanceMaterials)
                instantiateSlotsMaterial();

            if (initializeAtAwake)
                Initialize();
            else
                getDefaultEmote();
        }

        void Update()
        {
            // check emotion change in animation and update emotion if necessary
            if (!isEmoting && emoteAnimIndex != _currentEmoteAnimIndex)
                updateAnimEmote(emoteAnimIndex);
        }

        /// <summary>
        /// Initialize CharacterViewer according to the corresponded parts and settings.
        /// </summary>
        public void Initialize()
        {
            if (setupData == null)
                setupData = Resources.Load<SetupData>("CC2D_SetupData");

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
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = this.slots.GetSlot(cat);
                if (slot == null || slot.material == null)
                    continue;

                if (!materials.ContainsKey(slot.material.name))
                {
                    Material tmat = Instantiate<Material>(slot.material);
                    tmat.name = slot.material.name;
                    materials.Add(tmat.name, tmat);
                    slot.material = tmat;
                }
                else
                {
                    slot.material = materials[slot.material.name];
                }
            }
            RelinkMaterials();
        }

        /// <summary>
        /// Relink all materials used by this CharacterViewer to the value in each of its slots.
        /// </summary>
		public void RelinkMaterials()
        {
            relinkMaterials<SpriteRenderer>();
            relinkMaterials<SkinnedMeshRenderer>();
        }

        private void relinkMaterials<T>() where T : Renderer
        {
            T[] renderers = this.transform.GetComponentsInChildren<T>(true);
            foreach (T r in renderers)
            {
                if (r.sharedMaterial == null)
                    continue;

                PartSlot slot = slots.GetSlot(getMaterialsCategory(r.sharedMaterial));
                if (slot == null || slot.material == null)
                    continue;

                if (Application.isPlaying)
                    r.material = slot.material;
                else
                    r.sharedMaterial = slot.material;
            }
        }

        private SlotCategory getMaterialsCategory(Material material)
        {
            foreach (SlotCategory s in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = slots.GetSlot(s);
                if (slot == null || slot.material == null)
                    continue;

                if (material.name.Contains(slot.material.name))
                    return s;
            }

            foreach (MaterialData md in setupData.defaultMaterials)
            {
                if (material.name.Contains(md.material.name))
                    return md.slotCategory;
            }
            return SlotCategory.Nose;
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
            SortingGroup sortinggroup = this.GetComponent<SortingGroup>();
            sortinggroup.enabled = false;
            sortinggroup.enabled = true;
        }

        /// <summary>
        /// Repaint skin color according to the current value.
        /// </summary>
        public void RepaintSkinColor()
        {
            if (skins == null || skins.Count <= 0)
                return;

            foreach (SpriteRenderer s in skins)
            {
                if (s != null)
                    s.color = _skincolor;
            }
        }

        /// <summary>
        /// Repaint tint color according to the current value.
        /// </summary>
        public void RepaintTintColor()
        {
            foreach (SlotCategory c in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = slots.GetSlot(c);
                if (slot == null || slot.material == null)
                    continue;
                if (slot.material.HasProperty("_Color"))
                    slot.material.SetColor("_Color", _tintcolor);
            }
        }

        private void refreshPartSlots()
        {
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot s = this.slots.GetSlot(cat);
                if (s == null)
                    continue;
                EquipPart(cat, s.assignedPart);
                if (!SetupData.colorableSpriteLinks.ContainsKey(cat))
                {
                    SetPartColor(cat, ColorCode.Color1, s.color1);
                    SetPartColor(cat, ColorCode.Color2, s.color2);
                    SetPartColor(cat, ColorCode.Color3, s.color3);
                }
            }
        }

        /// <summary>
        /// Returns the assigned part of a given SlotCategory. Returns 'null' if there is no part assigned.
        /// </summary>
        /// <param name="category">Given SlotCategory.</param>
        /// <returns>Assigned part of the given SlotCategory if there is any, otherwise returns 'null'.</returns>
        public Part GetAssignedPart(SlotCategory category)
        {
            PartSlot slot = this.slots.GetSlot(category);
            if (slot == null)
                return null;
            return slot.assignedPart;
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
            //..find alternative if part doesn't support this character body type
            if (part != null && !part.supportedBody.Contains(this.bodyType))
            {
                Part altpart = getAlternatePart(part);
                if (altpart == null)
                    Debug.Log(String.Format("part '{0}' on slot category '{1}'  doesn't support body type {2}",
                        part, slotCategory, this.bodyType));
                else
                    Debug.Log(String.Format("part '{0}' on slot category '{1}'  doesn't support body type {2}. it will be replaced with {3}",
                        part, slotCategory, this.bodyType, altpart.name));
                part = altpart;
            }

            if (slotCategory == SlotCategory.MainHand)
            {
                Weapon mainweapon = (Weapon)part;
                Weapon offweapon = (Weapon)this.slots.GetSlot(SlotCategory.OffHand).assignedPart;
                if (mainweapon != null && mainweapon.weaponCategory == WeaponCategory.TwoHanded)
                    equipWeapon(SlotCategory.OffHand, null);
                else if (mainweapon != null && offweapon != null && offweapon.weaponCategory == WeaponCategory.Bow)
                    equipWeapon(SlotCategory.OffHand, null);
                else if (mainweapon != null && mainweapon.weaponCategory == WeaponCategory.Rifle)
                    equipWeapon(SlotCategory.OffHand, null);
                equipWeapon(SlotCategory.MainHand, mainweapon);
            }
            else if (slotCategory == SlotCategory.OffHand)
            {
                Weapon mainweapon = (Weapon)this.slots.GetSlot(SlotCategory.MainHand).assignedPart;
                Weapon offweapon = (Weapon)part;
                if (offweapon != null && offweapon.weaponCategory == WeaponCategory.Bow)
                    equipWeapon(SlotCategory.MainHand, null);
                else if (offweapon != null && mainweapon != null && mainweapon.weaponCategory == WeaponCategory.TwoHanded)
                    equipWeapon(SlotCategory.MainHand, null);
                else if (offweapon != null && mainweapon != null && mainweapon.weaponCategory == WeaponCategory.Rifle)
                    equipWeapon(SlotCategory.MainHand, null);
                equipWeapon(SlotCategory.OffHand, offweapon);
            }
            else if (slotCategory == SlotCategory.SkinDetails)
                equipSkinDetails(part);
            else if (slotCategory == SlotCategory.Cape)
                equipCape(part);
            else if (slotCategory == SlotCategory.Skirt)
                equipSkirt(part);
            else if (slotCategory == SlotCategory.Eyebrow || slotCategory == SlotCategory.Eyes || slotCategory == SlotCategory.Nose || slotCategory == SlotCategory.Mouth || slotCategory == SlotCategory.Ear)
            {
                equipPart(slotCategory, part);
                if (!isEmoting && !isEmotingAnimationEvent)
                    getDefaultEmote();
            }
            else
                equipPart(slotCategory, part);
        }

        private Part getAlternatePart(Part part)
        {
            List<Part> parts = PartList.Static.FindParts(part.category);
            int sid = parts.FindIndex(x => x == part);
            int inc = this.bodyType == BodyType.Male ? 1 : -1;

            for (int i = sid; (i >= 0) && (i < parts.Count); i += inc)
            {
                if (parts[i].supportedBody.Contains(this.bodyType))
                    return parts[i];
            }

            foreach (Part p in parts) //..search for any
            {
                if (p.supportedBody.Contains(this.bodyType))
                    return p;
            }

            return null;
        }

        private void equipWeapon(SlotCategory slotCategory, Weapon weapon)
        {
            PartSlot slot = this.slots.GetSlot(slotCategory);
            if (slot == null)
                return;

            resetWeaponRenderer(slot);
            if (weapon == null)
            {
                slot.assignedPart = null;
                return;
            }

            Dictionary<string, string> links = new Dictionary<string, string>();
            switch (slotCategory)
            {
                case SlotCategory.MainHand:
                    if (weapon.weaponCategory == WeaponCategory.OneHanded ||
                        weapon.weaponCategory == WeaponCategory.TwoHanded ||
                        weapon.weaponCategory == WeaponCategory.Gun ||
                        weapon.weaponCategory == WeaponCategory.Rifle)
                        links = SetupData.rWeaponLink;
                    break;
                case SlotCategory.OffHand:
                    if (weapon.weaponCategory == WeaponCategory.Bow)
                        links = SetupData.bowLink;
                    else if (weapon.weaponCategory == WeaponCategory.Shield)
                        links = SetupData.shieldLink;
                    else if (weapon.weaponCategory == WeaponCategory.OneHanded ||
                             weapon.weaponCategory == WeaponCategory.Gun)
                        links = SetupData.lWeaponLink;
                    break;
                default:
                    break;
            }

            slot.assignedPart = weapon;
            if (slot.material != null)
            {
                slot.material.SetTexture("_ColorMask", weapon.colorMask);
            }
            {
                foreach (Sprite s in weapon.sprites)
                    this.transform.Find(links[s.name]).GetComponent<SpriteRenderer>().sprite = s;
                ResourcesManager.ReleaseZeroReference();
            }
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                SetPartColor(slotCategory, ColorCode.Color1, slot.color1);
            Transform muzzlefx = null;
            if (slotCategory == SlotCategory.MainHand)
                muzzlefx = this.transform.Find(SetupData.muzzleFXLinks[SlotCategory.MainHand]);
            if (slotCategory == SlotCategory.OffHand)
                muzzlefx = this.transform.Find(SetupData.muzzleFXLinks[SlotCategory.OffHand]);
            muzzlefx.localPosition = weapon.muzzlePosition;
        }

        private void resetWeaponRenderer(PartSlot slot)
        {
            Weapon weapon = (Weapon)slot.assignedPart;
            if (weapon == null)
                return;

            if (slot == this.slots.GetSlot(SlotCategory.MainHand))
            {
                foreach (string k in SetupData.rWeaponLink.Keys)
                    this.transform.Find(SetupData.rWeaponLink[k]).GetComponent<SpriteRenderer>().sprite = null;
            }
            else if (slot == this.slots.GetSlot(SlotCategory.OffHand))
            {
                foreach (string k in SetupData.bowLink.Keys)
                    this.transform.Find(SetupData.bowLink[k]).GetComponent<SpriteRenderer>().sprite = null;
                foreach (string k in SetupData.shieldLink.Keys)
                    this.transform.Find(SetupData.shieldLink[k]).GetComponent<SpriteRenderer>().sprite = null;
                foreach (string k in SetupData.lWeaponLink.Keys)
                    this.transform.Find(SetupData.lWeaponLink[k]).GetComponent<SpriteRenderer>().sprite = null;
            }
        }

        private void equipSkinDetails(Part part)
        {
            if (part != null && part.category != PartCategory.SkinDetails)
                return;

            PartSlot slot = this.slots.GetSlot(SlotCategory.SkinDetails);
            if (slot == null)
                return;

            slot.assignedPart = part;
            if (slot.material != null)
            {
                if (part == null)
                    slot.material.SetTexture("_Details", null);
                else
                {
                    slot.material.SetTexture("_Details", part.texture);
                    ResourcesManager.ReleaseZeroReference();
                }
            }
        }

        private void equipCape(Part part)
        {
            if (part != null && part.category != PartCategory.Cape)
                return;

            PartSlot slot = this.slots.GetSlot(SlotCategory.Cape);
            if (slot == null)
                return;

            Transform capetransform = this.transform.Find("Root/Pos_Hip/Bone_Hip/Pos_Body/Bone_Body/Cape");
            slot.assignedPart = part;
            if (part == null)
            {
                if (slot.material != null)
                {
                    slot.material.SetTexture("_MainTex", null);
                    slot.material.SetTexture("_ColorMask", null);
                }
                if (capetransform != null)
                    capetransform.gameObject.SetActive(false);
            }
            else
            {
                if (slot.material != null)
                {
                    slot.material.SetTexture("_MainTex", part.texture);
                    slot.material.SetTexture("_ColorMask", part.colorMask);
                    ResourcesManager.ReleaseZeroReference();
                }
                if (capetransform != null)
                    capetransform.gameObject.SetActive(true);
            }
        }

        private void equipSkirt(Part part)
        {
            if (part != null && part.category != PartCategory.Skirt)
                return;

            PartSlot slot = this.slots.GetSlot(SlotCategory.Skirt);
            if (slot == null)
                return;

            Transform capetransform = this.transform.Find("Root/Pos_Hip/Bone_Hip/Skirt");
            slot.assignedPart = part;
            if (part == null)
            {
                if (slot.material != null)
                {
                    slot.material.SetTexture("_MainTex", null);
                    slot.material.SetTexture("_ColorMask", null);
                }
                if (capetransform != null)
                    capetransform.gameObject.SetActive(false);
            }
            else
            {
                if (slot.material != null)
                {
                    slot.material.SetTexture("_MainTex", part.texture);
                    slot.material.SetTexture("_ColorMask", part.colorMask);
                    ResourcesManager.ReleaseZeroReference();
                }
                if (capetransform != null)
                    capetransform.gameObject.SetActive(true);
            }
        }

        private void equipPart(SlotCategory slotCategory, Part part)
        {
            if (!SetupData.partLinks.ContainsKey(slotCategory))
                return;

            Dictionary<string, string> links = SetupData.partLinks[slotCategory];
            PartSlot slot = this.slots.GetSlot(slotCategory);

            if (links == null || slot == null)
                return;

            if (part != null && (int)slotCategory != (int)part.category)
            {
                Debug.Log("can't equip " + part.name + ". part doesn't match with slot category");
                return;
            }

            //..reset part
            foreach (string k in links.Keys)
                this.transform.Find(links[k]).GetComponent<SpriteRenderer>().sprite = null;

            if (part == null)
            {
                slot.assignedPart = null;
                if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                    SetPartColor(slotCategory, ColorCode.Color1, slot.color1);
                return;
            }

            slot.assignedPart = part;
            if (slot.material != null)
            {
                slot.material.SetTexture("_ColorMask", part.colorMask);
            }
            {
                foreach (Sprite s in part.sprites)
                    this.transform.Find(links[s.name]).GetComponent<SpriteRenderer>().sprite = s;
                ResourcesManager.ReleaseZeroReference();
            }
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
                SetPartColor(slotCategory, ColorCode.Color1, slot.color1);
        }

        /// <summary>
        /// Returns Part's Color from desired PartSlot.
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <returns>Part's Color. Returns 'Color.clear' if colorCode doesn't match with any value in 'CharacterCreator2D.ColorCode'.</returns>
        public Color GetPartColor(SlotCategory slotCategory, string colorCode)
        {
            PartSlot slot = slots.GetSlot(slotCategory);
            if (slot != null)
            {
                switch (colorCode)
                {
                    case ColorCode.Color1:
                        return slot.color1;
                    case ColorCode.Color2:
                        return slot.color2;
                    case ColorCode.Color3:
                        return slot.color3;
                    default:
                        return Color.clear;
                }
            }
            return Color.clear;
        }

        /// <summary>
        /// Modifies Part's Color. 
        /// </summary>
        /// <param name="slotCategory">Desired SlotCategory.</param>
        /// <param name="colorCode">Represent the desired color order of the part. Use 'CharacterCreator2D.ColorCode'.</param>
        /// <param name="color">Desired value of the color.</param>
        public void SetPartColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            if (slotCategory == SlotCategory.MainHand || slotCategory == SlotCategory.OffHand)
                setWeaponColor(slotCategory, colorCode, color);
            else if (slotCategory == SlotCategory.SkinDetails)
                setSkinDetailsColor(color);
            else
                setPartColor(slotCategory, colorCode, color);
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

        private void setWeaponColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            if (colorCode == ColorCode.Color1)
            {
                if (slotCategory == SlotCategory.MainHand)
                    this.transform.Find(SetupData.weaponFXLinks[SlotCategory.MainHand]).GetComponent<SpriteRenderer>().color = color;
                else if (slotCategory == SlotCategory.OffHand)
                    this.transform.Find(SetupData.weaponFXLinks[SlotCategory.OffHand]).GetComponent<SpriteRenderer>().color = color;
            }

            setPartColor(slotCategory, colorCode, color);
        }

        private void setSkinDetailsColor(Color color)
        {
            PartSlot slot = this.slots.GetSlot(SlotCategory.SkinDetails);
            slot.material.SetColor("_DetailsColor", color);
            slot.color1 = slot.color2 = slot.color3 = color;
        }

        private void setPartColor(SlotCategory slotCategory, string colorCode, Color color)
        {
            PartSlot slot = this.slots.GetSlot(slotCategory);
            if (SetupData.colorableSpriteLinks.ContainsKey(slotCategory))
            {
                List<string> links = SetupData.colorableSpriteLinks[slotCategory];
                foreach (string l in links)
                    this.transform.Find(l).GetComponent<SpriteRenderer>().color = color;
                slot.color1 = slot.color2 = slot.color3 = color;
                return;
            }

            if (slot == null || slot.material == null)
                return;

            slot.material.SetColor(colorCode, color);
            switch (colorCode)
            {
                case ColorCode.Color1:
                    slot.color1 = color;
                    break;
                case ColorCode.Color2:
                    slot.color2 = color;
                    break;
                case ColorCode.Color3:
                    slot.color3 = color;
                    break;
                default:
                    break;
            }
        }

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
            if (!this.gameObject.activeInHierarchy)
                return;

            Transform bodyobj = setupData.GetBodyPrefab(bodyType);
            if (bodyobj == null)
                return;

            detachEntities();
            Animator animator = this.GetComponent<Animator>();
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
            Transform oldbody = this.transform.Find("Root");
            if (oldbody != null)
                DestroyImmediate(oldbody.gameObject);
            bodyobj = Instantiate(bodyobj, this.transform.position, this.transform.rotation, this.transform);
            bodyobj.name = "Root";
            animator.Rebind();
            animator.Play(curranimBase, 0);
            animator.Play(curranimAim, 1);
            animator.SetFloat("Aim", currAim);
            _bodyscaler.Initialize(this);
            attachEntities();
        }

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

        /// <summary>
        /// Change character facial expression/emote.
        /// </summary>
        /// <param name="emotionType">target emotion.</param>
        public void Emote(EmotionType emotionType)
        {
            Emote(emotionType, 0f);
        }

        /// <summary>
        /// Change character facial expression/emote then reset to default face after a period of time.
        /// </summary>
        /// <param name="emotionType">target emotion.</param>
        /// <param name="duration">the duration of the emotion before resetting back to default.</param>
        public void Emote(EmotionType emotionType, float duration)
        {
            EmoteIndex i = emotes.getIndex(emotionType);
            if (isEmotingAnimationEvent) updateAnimEmote(-1);
            emote(i, duration, false);
        }

        /// <summary>
        /// Change character facial expression/emote.
        /// </summary>
        /// <param name="emotionName">target emotion name.</param>
        public void Emote(string emotionName)
        {
            Emote(emotionName, 0f);
        }

        /// <summary>
        /// Change character facial expression/emote then reset to default face after a period of time.
        /// </summary>
        /// <param name="emotionName">target emotion name.</param>
        /// <param name="duration">the duration of the emotion before resetting back to default.</param>
        public void Emote(string emotionName, float duration)
        {
            EmoteIndex i = null;
            foreach (EmotionType emotionType in Enum.GetValues(typeof(EmotionType)))
            {
                EmoteIndex e = emotes.getIndex(emotionType);
                if (e.name == emotionName)
                {
                    i = e;
                    break;
                }
            }
            if (i == null)
            {
                Debug.LogError("Emotion not found: " + emotionName);
                return;
            }
            if (isEmotingAnimationEvent) updateAnimEmote(-1);
            emote(i, duration, false);
        }

        private void emote(EmoteIndex i, float duration, bool isAnimation)
        {
            CancelInvoke("ResetEmote");
            isEmoting = !isAnimation;
            if (i.eyebrowPart != null)
                EquipPart(SlotCategory.Eyebrow, i.eyebrowPart);
            else
                EquipPart(SlotCategory.Eyebrow, defaultEmote.eyebrowPart);
            if (i.eyesPart != null)
                EquipPart(SlotCategory.Eyes, i.eyesPart);
            else
                EquipPart(SlotCategory.Eyes, defaultEmote.eyesPart);
            if (i.nosePart != null)
                EquipPart(SlotCategory.Nose, i.nosePart);
            else
                EquipPart(SlotCategory.Nose, defaultEmote.nosePart);
            if (i.mouthPart != null)
                EquipPart(SlotCategory.Mouth, i.mouthPart);
            else
                EquipPart(SlotCategory.Mouth, defaultEmote.mouthPart);
            if (i.earPart != null)
                EquipPart(SlotCategory.Ear, i.earPart);
            else
                EquipPart(SlotCategory.Ear, defaultEmote.earPart);
            if (duration > 0)
                Invoke("ResetEmote", duration);
        }

        /// <summary>
        /// Reset character facial expression/emote into its default face.
        /// </summary>
        public void ResetEmote()
        {
            EmoteIndex i = defaultEmote;
            EquipPart(SlotCategory.Eyebrow, i.eyebrowPart);
            EquipPart(SlotCategory.Eyes, i.eyesPart);
            EquipPart(SlotCategory.Nose, i.nosePart);
            EquipPart(SlotCategory.Mouth, i.mouthPart);
            EquipPart(SlotCategory.Ear, i.earPart);
            isEmoting = false;
        }

        private void getDefaultEmote()
        {
            Part p;
            p = GetAssignedPart(SlotCategory.Eyebrow);
            defaultEmote.eyebrowPart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Eyes);
            defaultEmote.eyesPart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Nose);
            defaultEmote.nosePart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Mouth);
            defaultEmote.mouthPart = p == null ? null : p;
            p = GetAssignedPart(SlotCategory.Ear);
            defaultEmote.earPart = p == null ? null : p;
        }

        private void updateAnimEmote(float i)
        {
            int index = (int)i;
            if (index >= 0)
            {
                ResetEmote();
                isEmotingAnimationEvent = true;
                EmoteIndex e = emotes.getIndex((EmotionType)index);
                emote(e, 0f, true);
            }
            else
            {
                ResetEmote();
                isEmotingAnimationEvent = false;
            }
            _currentEmoteAnimIndex = emoteAnimIndex;
        }

        #region JSON
        /// <summary>
        /// Generate CharacterData of CharacterViewer.
        /// </summary>
        /// <returns>Generated CharacterData of CharacterViewer.</returns>
        public CharacterData GenerateCharacterData()
        {
            CharacterData val = new CharacterData();
            val.dataVersion = setupData.dataVersion;
            val.bodyType = this.bodyType;
            val.skinColor = _skincolor;
            val.tintColor = _tintcolor;

            val.slotData = new List<PartSlotData>();
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = this.slots.GetSlot(cat);
                val.slotData.Add(new PartSlotData()
                {
                    category = cat,
                    partName = slot.assignedPart == null ? "" : slot.assignedPart.name,
                    partPackage = slot.assignedPart == null ? "" : slot.assignedPart.packageName,
                    color1 = slot.color1,
                    color2 = slot.color2,
                    color3 = slot.color3
                });
            }

            val.emoteData = new List<EmoteIndexData>();
            foreach (EmotionType emotionType in Enum.GetValues(typeof(EmotionType)))
            {
                EmoteIndex e = emotes.getIndex(emotionType);
                if (e == null) continue;
                if (emotionType >= EmotionType.Blink && String.IsNullOrEmpty(e.name)) continue;
                else val.emoteData.Add(new EmoteIndexData()
                {
                    emotionType = emotionType,
                    emotionName = String.IsNullOrEmpty(e.name) ? "" : e.name,
                    eyebrowPartName = e.eyebrowPart == null ? "" : e.eyebrowPart.name,
                    eyebrowPackage = e.eyebrowPart == null ? "" : e.eyebrowPart.packageName,
                    eyesPartName = e.eyesPart == null ? "" : e.eyesPart.name,
                    eyesPackage = e.eyesPart == null ? "" : e.eyesPart.packageName,
                    nosePartName = e.nosePart == null ? "" : e.nosePart.name,
                    nosePackage = e.nosePart == null ? "" : e.nosePart.packageName,
                    mouthPartName = e.mouthPart == null ? "" : e.mouthPart.name,
                    mouthPackage = e.mouthPart == null ? "" : e.mouthPart.packageName,
                    earPartName = e.earPart == null ? "" : e.earPart.name,
                    earPackage = e.earPart == null ? "" : e.earPart.packageName
                });
            }

            //..update
            val.bodySegmentData = new List<SegmentScaleData>();
            foreach (SegmentType stype in Enum.GetValues(typeof(SegmentType)))
            {
                Vector2 scale = GetBodySlider(stype);
                val.bodySegmentData.Add(new SegmentScaleData()
                {
                    segmentType = stype,
                    scale = new Vector2(scale.x, scale.y)
                });
            }
            //update..

            return val;
        }

        /// <summary>
        /// Assign and initialize this CharacterViewer according to a given CharacterData.
        /// </summary>
        /// <param name="data">CharacterData to be assigned from.</param>
        public void AssignCharacterData(CharacterData data)
        {
            //data version exception..
            if (data.dataVersion < 1)
            {
                data.slotData.Add(new PartSlotData()
                {
                    category = SlotCategory.BodySkin,
                    partName = data.bodyType == BodyType.Male ? "Base 00 Male" : "Base 00 Female",
                    partPackage = "Base"
                });
            }
            //..data version exception

            this.bodyType = data.bodyType;
            _skincolor = data.skinColor;
            _tintcolor = data.tintColor;

            Initialize();
            foreach (SlotCategory cat in Enum.GetValues(typeof(SlotCategory)))
            {
                PartSlot slot = this.slots.GetSlot(cat);
                PartSlotData slotdata = data.slotData.Find(x => x.category == cat);
                if (slot == null)
                    continue;
                
                if (string.IsNullOrEmpty(slotdata.partName))
                {
                    EquipPart(cat, (Part)null);
                    continue;
                }

                Part part = PartList.Static.FindPart(slotdata.partName, slotdata.partPackage, cat);
                if (part == null)
                    continue;

                EquipPart(cat, part);
                SetPartColor(cat, ColorCode.Color1, slotdata.color1);
                SetPartColor(cat, ColorCode.Color2, slotdata.color2);
                SetPartColor(cat, ColorCode.Color3, slotdata.color3);
            }

            if (data.emoteData != null || data.emoteData.Count > 0)
            {
                foreach (EmotionType emotionType in Enum.GetValues(typeof(EmotionType)))
                {
                    EmoteIndex e = emotes.getIndex(emotionType);
                    EmoteIndexData edata = data.emoteData.Find(x => x.emotionType == emotionType);
                    if (e == null)
                        continue;
                    if (emotionType >= EmotionType.Blink && String.IsNullOrEmpty(edata.emotionName))
                        continue;
                    e.name = edata.emotionName;
                    if (string.IsNullOrEmpty(edata.eyebrowPartName))
                        e.eyebrowPart = null;
                    else
                        e.eyebrowPart = PartList.Static.FindPart(edata.eyebrowPartName, edata.eyebrowPackage, SlotCategory.Eyebrow);
                    if (string.IsNullOrEmpty(edata.eyesPartName))
                        e.eyesPart = null;
                    else
                        e.eyesPart = PartList.Static.FindPart(edata.eyesPartName, edata.eyesPackage, SlotCategory.Eyes);
                    if (string.IsNullOrEmpty(edata.nosePartName))
                        e.nosePart = null;
                    else
                        e.nosePart = PartList.Static.FindPart(edata.nosePartName, edata.nosePackage, SlotCategory.Nose);
                    if (string.IsNullOrEmpty(edata.mouthPartName))
                        e.mouthPart = null;
                    else
                        e.mouthPart = PartList.Static.FindPart(edata.mouthPartName, edata.mouthPackage, SlotCategory.Mouth);
                    if (string.IsNullOrEmpty(edata.earPartName))
                        e.earPart = null;
                    else
                        e.earPart = PartList.Static.FindPart(edata.earPartName, edata.earPackage, SlotCategory.Ear);
                }
            }

            //..update
            if (data.bodySegmentData == null || data.bodySegmentData.Count <= 0)
            {
                foreach (SegmentType st in Enum.GetValues(typeof(SegmentType)))
                    SetBodySlider(st, new Vector2(0.5f, 0.5f));
            }
            else
            {
                foreach (SegmentScaleData sd in data.bodySegmentData)
                    SetBodySlider(sd.segmentType, sd.scale);
            }
            //update..
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
                    directory = Directory.GetCurrentDirectory();
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
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
                Debug.LogWarning("load CharacterViewer '" + this.name + "' from JSON's failed: file doesn't exist");
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
        #endregion

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
        #endregion
    }
}