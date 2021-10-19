using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class UICreator : MonoBehaviour
    {
    // The following code is used for the UI function such as updating, opening and closing menus, etc.
    #region UI and Menus

        /// <summary>
        /// CharacterViewer displayed by this UICreator.
        /// </summary>
        [Tooltip("CharacterViewer displayed by this UICreator")]
        public CharacterViewer character;

        /// <summary>
        /// UIColor managed by this UICreator.
        /// </summary>
        [Tooltip("UIColor managed by this UICreator")]
        public UIColor colorUI;

        /// <summary>
        /// RuntimeDialog used by this UICreator.
        /// </summary>
        [Tooltip("RuntimeDialog used by this UICreator")]
        public RuntimeDialog dialog;

        private List<GameObject> menus;
        private List<GameObject> partmenus;
        private UIPartColor uipartcolor;
        [HideInInspector]
        public CharacterData defaultCharacter;
#if UNITY_EDITOR
        public static System.Action onBakeCharacterAsPrefab; 
#endif

        void Awake()
        {
            _processing = false;
            BodyTypeGroup bodygroup = this.transform.GetComponentInChildren<BodyTypeGroup>(true);
            bodygroup.Initialize();
            PartGroup[] partgroups = this.transform.GetComponentsInChildren<PartGroup>(true);
            foreach (PartGroup g in partgroups)
                g.Initialize();
            menus = new List<GameObject>();
            foreach (Transform child in this.transform.Find("Menus"))
                menus.Add(child.gameObject);
            partmenus = new List<GameObject>();
            foreach (Transform child in this.transform.Find("Parts"))
                partmenus.Add(child.gameObject);
            defaultCharacter = character.GenerateCharacterData();
        }

        void CloseAllMenus () 
        {
            if (menus != null)
            foreach (GameObject t in menus) 
            {
                t.gameObject.SetActive(false);
            }
            if (partmenus != null)
            foreach (GameObject t in partmenus)
            {
                t.gameObject.SetActive(false);
            }
        }

        public void OpenMenu (string title)
        {
            CloseAllMenus();
            findMenu(title).SetActive(true);
        }

        public void OpenPartMenu (string title, SlotCategory slotCategory)
        {
            OpenMenu(title);
            if (uipartcolor == null)
                uipartcolor = findMenu("Parts Color").GetComponent<UIPartColor>();
            uipartcolor.Initialize(slotCategory);
            uipartcolor.gameObject.SetActive(true);
        }

        GameObject findMenu (string title)
        {
            if (menus != null)
            foreach (GameObject t in menus) 
            {
                if (t.name == title)
                return t;
            }
            if (partmenus != null)
            foreach (GameObject t in partmenus)
            {
                if (t.name == title)
                return t;
            }
            return null;
        }

        public void NewCharacter () 
        {
            dialog.DisplayDialog("New Character", "Are you sure you want to create a new character? All unsaved changes will be lost.", true);	
            dialog.yesButton.onClick.AddListener(newCharacter);
        }

        void newCharacter ()
        {
			character.AssignCharacterData(defaultCharacter);
        }

        public void OpenDocumentation () 
        {
            Application.OpenURL("http://bit.ly/CC2Ddoc");
        }

    #endregion
        
    // The following code is responsible for saving and loading character in the UI.
    #region Saving and Loading

        #if !UNITY_EDITOR
        /// <summary>
        /// The default path that are used when saving or loading character as JSON during runtime.
        /// </summary>
        string RuntimeSavePath = "Characters/character.json";
        #endif
        
        private bool _processing = false;

        /// <summary>
        /// [EDITOR] Save character as a prefab in desired path.
        /// </summary>
        public void SaveCharacterAsPrefab()
        {
            if (this.character == null || _processing)
                return;
            StartCoroutine("ie_savecharaasprefab");
        }

        IEnumerator ie_savecharaasprefab()
        {
            _processing = true;
            yield return null;
        #if UNITY_EDITOR
            string path = CharacterUtils.ShowSaveFileDialog("Save Character", "New Character", "prefab", true);
            if (!string.IsNullOrEmpty(path))
            {
                CharacterViewer tcharacter = CharacterUtils.SpawnCharacter(this.character, path);
                yield return null;
                yield return null;
                CharacterUtils.SaveCharacterToPrefab(tcharacter, path);
                yield return null;
                yield return null;
                Destroy(tcharacter.gameObject);
                dialog.DisplayDialog("Save Character", "Character is saved to <color=#178294>" + path + "</color> succesfully.");
            }
        #endif
            _processing = false;
        }

        /// <summary>
        /// [EDITOR] Load character from a prefab.
        /// </summary>
        public void LoadCharacterFromPrefab()
        {
#if UNITY_EDITOR
            _processing = true;
            string path = CharacterUtils.ShowOpenFileDialog("Load Character", "prefab", true);
            if (!string.IsNullOrEmpty(path))
            {
                CharacterViewer tcharacter = CharacterUtils.LoadCharacterFromPrefab(path);
                if (tcharacter == null)
                    return;

                CharacterData data = tcharacter.GenerateCharacterData();
                this.character.AssignCharacterData(data);
                dialog.DisplayDialog("Load Character", "Character from <color=#178294>" + path + "</color> is loaded succesfully.");
            }
            _processing = false;
#endif        
        }

        /// <summary>
        /// Save character's data as JSON file. Calling this function will save character's data with path defined in JSONRuntimePath field.
        /// </summary>
        public void SaveCharacterToJSON()
        {
            _processing = true;
            string path = "";

        #if UNITY_EDITOR
            path = CharacterUtils.ShowSaveFileDialog("Save Character", "New Character Data", "json", false);
        #else
			path = this.RuntimeSavePath;
        #endif

            if (!string.IsNullOrEmpty(path))
            {
                this.character.SaveToJSON(path);
                dialog.DisplayDialog("Save Character", "Character is saved to <color=#178294>" + path + "</color> succesfully.");

        #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
        #endif
        
            }
            _processing = false;
        }

        /// <summary>
        /// Load character from JSON file's data. Calling this function will try to load character's data from a path defined in JSONRuntimePath field.
        /// </summary>
        public void LoadCharacterFromJSON()
        {
            _processing = true;
            string path = "";

        #if UNITY_EDITOR
            path = CharacterUtils.ShowOpenFileDialog("Load Character", "json", false);
        #else
			path = this.RuntimeSavePath;
        #endif

            if (!string.IsNullOrEmpty(path))
            {
                this.character.LoadFromJSON(path);
                dialog.DisplayDialog("Load Character", "Character from <color=#178294>" + path + "</color> is loaded succesfully.");
            }
            _processing = false;
        }

        /// <summary>
        /// Bake character to prefab. Editor only.
        /// </summary>
        public void BakeCharacterAsPrefab()
        {
#if UNITY_EDITOR
            onBakeCharacterAsPrefab?.Invoke(); 
#endif
        }
    #endregion
        
    // The following code is used for the randomizer in the UI.
    #region Randomizer
        
        private Dictionary<SlotCategory, List<Part>> _availparts = new Dictionary<SlotCategory, List<Part>>();
        private Part nullpart = null;

        /// <summary>
        /// Probability setting for the Randomize function
        /// </summary>
        private readonly Dictionary<SlotCategory, int> _RANDSETTINGS = new Dictionary<SlotCategory, int>()
        {
            { SlotCategory.Armor, 100 },
            { SlotCategory.Boots, 100 },
            { SlotCategory.Cape, 40 },
            { SlotCategory.Ear, 40 },
            { SlotCategory.Eyebrow, 98 },
            { SlotCategory.Eyes, 100 },
            { SlotCategory.FacialHair, 50 },
            { SlotCategory.Gloves, 60 },
            { SlotCategory.Hair, 90 },
            { SlotCategory.Helmet, 70 },
            { SlotCategory.MainHand, 50 },
            { SlotCategory.Mouth, 100 },
            { SlotCategory.Nose, 100 },
            { SlotCategory.OffHand, 50 },
            { SlotCategory.Pants, 100 },
            { SlotCategory.SkinDetails, 40 },
            { SlotCategory.Skirt, 20 },
            { SlotCategory.BodySkin, 20}
        };

        /// <summary>
        /// Randomize character's body
        /// </summary>
        public void RandomizeBody()
        {
            character.SetBodyType(RollDice() > 50 ? BodyType.Male : BodyType.Female);
        }

        /// <summary>
        /// Randomize character's skin color
        /// </summary>
        public void RandomizeSkinColor()
        {
            List<Color> skins = GetColors("Skin");
            if (RollDice() > 80)
                skins = GetColors("Bleached");
            character.SkinColor = skins[Random.Range(0, skins.Count)];
        }

        /// <summary>
        /// Randomize character's body sliders
        /// </summary>
        public void RandomizeBodySliders()
        {
            BodySegmentSlider[] sliders = this.transform.GetComponentsInChildren<BodySegmentSlider>(true);
            foreach (BodySegmentSlider s in sliders)
                s.RandomizeScale();
        }

        /// <summary>
        /// Reset all character's body sliders
        /// </summary>
        public void ResetBodySliders()
        {
            BodySegmentSlider[] sliders = this.transform.GetComponentsInChildren<BodySegmentSlider>(true);
            foreach (BodySegmentSlider s in sliders)
                s.ResetScale();
        }

        /// <summary>
        /// Randomize character's part.
        /// </summary>
        public void RandomizePart(SlotCategory slotCategory)
        {
            if (_availparts == null || _availparts.Count <= 0)
            {
                _availparts = getAvailableParts(new List<string>());
            }
            character.EquipPart(slotCategory, _availparts[slotCategory][Random.Range(0, _availparts[slotCategory].Count)]);
        }

        /// <summary>
        /// Randomize character's weapon.
        /// </summary>
        public void RandomizeWeapon(SlotCategory slotCategory, WeaponCategory weaponCategory)
        {
            if (_availparts == null || _availparts.Count <= 0)
            {
                _availparts = getAvailableParts(new List<string>());
            }
            List<Part> weaponlist = new List<Part>();
            foreach (Part p in _availparts[slotCategory])
            {
                if (p is Weapon)
                {
                    Weapon w = (Weapon)p;
                    if (w.weaponCategory == weaponCategory)
                        weaponlist.Add(w);
                }
            }
            character.EquipPart(slotCategory, weaponlist[Random.Range(0,weaponlist.Count)]);
        }

        /// <summary>
        /// Randomize character's parts
        /// </summary>
        public void RandomizePart(List<SlotCategory> excludedCategory, List<string> excludedPacks)
        {
            List<SlotCategory> excludedcats = new List<SlotCategory>(excludedCategory);
            Dictionary<SlotCategory, List<Part>> availparts = getAvailableParts(excludedPacks);

            // Equip
            foreach (SlotCategory c in System.Enum.GetValues(typeof(SlotCategory)))
            {
                if (excludedcats.Contains(c) || availparts[c] == null || availparts[c].Count <= 0)
                    continue;

                switch (c)
                {
                    case SlotCategory.OffHand:
                        Weapon w = character.GetAssignedPart(SlotCategory.MainHand) as Weapon;
                        if (w == null)
                            character.EquipPart(c, nullpart);
                        else if (w.weaponCategory != WeaponCategory.TwoHanded && RollDice() < _RANDSETTINGS[c])
                            character.EquipPart(c, availparts[c][Random.Range(0, availparts[c].Count)]);
                        break;
                    case SlotCategory.Ear:
                        if (RollDice() < _RANDSETTINGS[c] || !excludedcats.Contains(c))
                            character.EquipPart(c, availparts[c][Random.Range(0, availparts[c].Count)]);
                        else
                            character.EquipPart(c, "00", "Base");
                        break;
                    case SlotCategory.BodySkin:
                        if (RollDice() < _RANDSETTINGS[c] || !excludedcats.Contains(c))
                            character.EquipPart(c, availparts[c][Random.Range(0, availparts[c].Count)]);
                        else
                            character.EquipPart(c, "Base 00 Male");
                        break;
                    case SlotCategory.Skirt:
                        int pskirt = character.bodyType == BodyType.Female ? 40 : 20;
                        if (RollDice() < pskirt)
                        {
                            character.EquipPart(c, availparts[c][Random.Range(0, availparts[c].Count)]);
                            if (RollDice() < pskirt)
                                character.EquipPart(SlotCategory.Pants, "Base 00 Male");
                        }
                        else
                        {
                            character.EquipPart(c, nullpart);
                        }
                        break;
                    default:
                        character.EquipPart(c, RollDice() < _RANDSETTINGS[c] ? availparts[c][Random.Range(0, availparts[c].Count)] : nullpart);
                        break;
                }
            }
        }

        // Get all available parts if null
        Dictionary<SlotCategory, List<Part>> getAvailableParts(List<string> excludedPacks)
        {
            Dictionary<SlotCategory, List<Part>> val = new Dictionary<SlotCategory, List<Part>>();
            foreach (SlotCategory c in System.Enum.GetValues(typeof(SlotCategory)))
                val.Add(c, PartList.Static.FindParts(c).FindAll(x => !excludedPacks.Contains(x.packageName)));
            return val;
        }

        /// <summary>
        /// Randomize color of character's part
        /// </summary>
        public void RandomizeColor(SlotCategory slotCategory)
        {
			List<SlotCategory> excludedCategory = new List<SlotCategory>();
			foreach (SlotCategory c in System.Enum.GetValues(typeof(SlotCategory)))
				if (c != slotCategory)
					excludedCategory.Add(c);
			RandomizeColor(excludedCategory.ToArray());
        }

        /// <summary>
        /// Randomize color(s) of character's part(s)
        /// </summary>
        /// <param name="excludedCategory">(optional) Excluded SlotCategory</param>
        public void RandomizeColor(params SlotCategory[] excludedCategory)
        {
            List<SlotCategory> excludedcats = new List<SlotCategory>(excludedCategory);
            // Roll flashiness & variant
            int flashiness = RollDice();
            int variant = RollDice();
            List<Color> colorpool = new List<Color>();
            List<Color> colortheme = new List<Color>();
            List<Color> haircolor = GetColors("Hair");
            List<Color> lipscolor = GetColors("Lips");
            List<Color> tattcolor = GetColors("Grayscale");
            List<Color> eyecolor = GetColors("Eyes");
            colorpool.AddRange(GetColors("Bleak"));
            colorpool.AddRange(GetColors("Dark"));
            colorpool.AddRange(GetColors("Leather"));
            colorpool.AddRange(GetColors("Grayscale"));
            lipscolor.AddRange(GetColors("Skin"));
            tattcolor.AddRange(GetColors("Bleak"));
            tattcolor.AddRange(GetColors("Fabric"));
            tattcolor.AddRange(GetColors("Vibrant"));
            if (flashiness >= 20) // Bleak and dark
            {
                colorpool.AddRange(GetColors("Metal"));
                colorpool.AddRange(GetColors("Bleached"));
                haircolor.AddRange(GetColors("Metal"));
                haircolor.AddRange(GetColors("Eyes"));
                lipscolor.AddRange(GetColors("Bleached"));
            }
            if (flashiness >= 50) // Somewhat normal
            {
                colorpool.AddRange(GetColors("Fabric"));
                haircolor.AddRange(GetColors("Fabric"));
                haircolor.AddRange(GetColors("Vibrant"));
                lipscolor.AddRange(GetColors("Fabric"));
                lipscolor.AddRange(GetColors("Bleak"));
                lipscolor.AddRange(GetColors("Dark"));
            }
            if (flashiness >= 80) // Flashy af
            {
                colorpool.AddRange(GetColors("Vibrant"));
                haircolor.AddRange(GetColors("Bleached"));
                lipscolor.AddRange(GetColors("Vibrant"));
            }
            if (variant <= 30)
                for (int i = 0; i < 6; i++)
                    colortheme.Add(colorpool[Random.Range(0, colorpool.Count)]);
            else if (variant > 30 && variant < 80)
                for (int i = 0; i < 9; i++)
                    colortheme.Add(colorpool[Random.Range(0, colorpool.Count)]);
            else if (variant >= 80)
                for (int i = 0; i < 12; i++)
                    colortheme.Add(colorpool[Random.Range(0, colorpool.Count)]);
            if (RollDice() > 60)
                eyecolor.AddRange(GetColors("Vibrant"));
            // Set equipment colors
            if (!excludedcats.Contains(SlotCategory.Armor))
                character.SetPartColor(SlotCategory.Armor, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            if (!excludedcats.Contains(SlotCategory.Pants))
                character.SetPartColor(SlotCategory.Pants, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            if (!excludedcats.Contains(SlotCategory.Helmet))
                character.SetPartColor(SlotCategory.Helmet, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            if (!excludedcats.Contains(SlotCategory.Gloves))
                character.SetPartColor(SlotCategory.Gloves, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            if (!excludedcats.Contains(SlotCategory.Boots))
                character.SetPartColor(SlotCategory.Boots, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            if (!excludedcats.Contains(SlotCategory.Cape))
                character.SetPartColor(SlotCategory.Cape, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            if (!excludedcats.Contains(SlotCategory.MainHand))
                character.SetPartColor(SlotCategory.MainHand, colorpool[Random.Range(0, colorpool.Count)], colorpool[Random.Range(0, colorpool.Count)], colorpool[Random.Range(0, colorpool.Count)]);
            if (!excludedcats.Contains(SlotCategory.OffHand))
                character.SetPartColor(SlotCategory.OffHand, colorpool[Random.Range(0, colorpool.Count)], colorpool[Random.Range(0, colorpool.Count)], colorpool[Random.Range(0, colorpool.Count)]);
            if (!excludedcats.Contains(SlotCategory.Skirt))
                character.SetPartColor(SlotCategory.Skirt, colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)], colortheme[Random.Range(0, colortheme.Count)]);
            // Set face colors
            Color newcolor = haircolor[Random.Range(0, haircolor.Count)];
            if (!excludedcats.Contains(SlotCategory.Hair))
                character.SetPartColor(SlotCategory.Hair, ColorCode.Color1, newcolor);
            if (!excludedcats.Contains(SlotCategory.FacialHair))
                character.SetPartColor(SlotCategory.FacialHair, ColorCode.Color1, newcolor);
            if (!excludedcats.Contains(SlotCategory.Eyebrow))
            {
                if (flashiness < 80)
                    character.SetPartColor(SlotCategory.Eyebrow, ColorCode.Color1, newcolor);
                else
                    character.SetPartColor(SlotCategory.Eyebrow, ColorCode.Color1, haircolor[Random.Range(0, haircolor.Count)]);
            }
            if (!excludedcats.Contains(SlotCategory.Eyes))
                character.SetPartColor(SlotCategory.Eyes, ColorCode.Color1, eyecolor[Random.Range(0, eyecolor.Count)]);
            if (!excludedcats.Contains(SlotCategory.Mouth))
            {
                if (character.bodyType == BodyType.Female)
                    character.SetPartColor(SlotCategory.Mouth, ColorCode.Color1, lipscolor[Random.Range(0, lipscolor.Count)]);
                else
                    character.SetPartColor(SlotCategory.Mouth, ColorCode.Color1, character.SkinColor);
            }
            if (!excludedcats.Contains(SlotCategory.SkinDetails))
            {
                if (character.GetAssignedPart(SlotCategory.SkinDetails) != null)
                {
                    newcolor = tattcolor[Random.Range(0, tattcolor.Count)];
                    if (RollDice() > 50) newcolor.a = Random.Range(0.2f, 1f);
                }
                else newcolor = Color.gray;
                character.SetPartColor(SlotCategory.SkinDetails, ColorCode.Color1, newcolor);
            }
        }

        int RollDice ()
        {
            int i = Random.Range(0,100);
            return i;
        }

        List<Color> GetColors (string palette)
        {
            List<Color> colors = new List<Color>();
            foreach (ColorPalette p in this.colorUI.colorPalette.colorPalettes)
            {
                if (p.paletteName == palette)
                {
                    foreach (Color c in p.colors) colors.Add(c);
                    break;
                }
            }
            return colors;
        }
    }
#endregion

}