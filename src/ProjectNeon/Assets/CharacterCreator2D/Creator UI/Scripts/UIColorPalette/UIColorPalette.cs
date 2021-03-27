using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class UIColorPalette : MonoBehaviour
    {
        /// <summary>
        /// PackageItem's template.
        /// </summary>
        [Tooltip("PackageItem's template")]
        public PackageItem tPackageItem;

        /// <summary>
        /// PaletteItem's template.
        /// </summary>
        [Tooltip("PaletteItem's template")]
        public PaletteItem tPaletteItem;

        /// <summary>
        /// JSON file location of this customPalette in Build mode.
        /// </summary>
        [Tooltip("JSON file location of this customPalette in Build mode")]
        public string JSONRuntimePath;

        /// <summary>
        /// Current color selected by this UIColorPalette.
        /// </summary>
        [Tooltip("Current color selected by this UIColorPalette")]
        [ReadOnly]
        public Color color = Color.gray;

        /// <summary>
        /// Custom ColorPalette provided for the users.
        /// </summary>
        [Tooltip("Custom ColorPalette provided for the users")]
        public ColorPalette customPalette;

        /// <summary>
        /// All ColorPalettes that will be loaded by this UIColorPalette.
        /// </summary>
        [Tooltip("All ColorPalettes that will be loaded by this UIColorPalette")]
        public List<ColorPalette> colorPalettes;

        private PaletteItem _customitem;

        void Awake()
        {
            initialize();
        }

        private void initialize()
        {
#if !UNITY_EDITOR
            customPalette.LoadFromJSON(JSONRuntimePath);
#endif
            clearPalettes();
            spawnPalettes();
        }

        private void clearPalettes()
        {
            PackageItem[] tpackages = this.transform.GetComponentsInChildren<PackageItem>(true);
            for (int i = 0; i < tpackages.Length; i++)
            {
                GameObject go = tpackages[i].gameObject;
                tpackages[i] = null;
                Destroy(go);
            }

            PaletteItem[] tpalettes = this.transform.GetComponentsInChildren<PaletteItem>(true);
            for (int i = 0; i < tpackages.Length; i++)
            {
                GameObject go = tpalettes[i].gameObject;
                tpalettes[i] = null;
                Destroy(go);
            }
        }

        private void spawnPalettes()
        {
            Transform itemparent = this.transform.Find("Content");
            if (itemparent == null)
                itemparent = this.transform;

            foreach (ColorPalette p in this.colorPalettes)
            {
                PackageItem tpackage = Instantiate<PackageItem>(tPackageItem, itemparent);
                tpackage.Initialize(p.paletteName);
                PaletteItem tpalette = Instantiate<PaletteItem>(tPaletteItem, itemparent);
                tpalette.Initialize(p);
            }

            if (customPalette != null)
            {
                PackageItem cpackage = Instantiate<PackageItem>(tPackageItem, itemparent);
                cpackage.Initialize(customPalette.paletteName);
                _customitem = Instantiate<PaletteItem>(tPaletteItem, itemparent);
                _customitem.Initialize(customPalette, true);
            }
        }

        /// <summary>
        /// Add a given Color to customPalette.
        /// </summary>
        /// <param name="color">Color to be added.</param>
        public void AddToCustomPalette(Color color)
        {
            if (customPalette == null || _customitem == null)
                return;

            customPalette.colors.Add(color);
            _customitem.Initialize(customPalette, true);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(customPalette);
#else
            customPalette.SaveToJSON(JSONRuntimePath);
#endif
        }

        /// <summary>
        /// Remove a given Color from customPalette.
        /// </summary>
        /// <param name="color">Color to be removed.</param>
        public void RemoveFromCustomPalette(Color color)
        {
            if (customPalette == null || _customitem == null)
                return;

            if (customPalette.colors.Contains(color))
            {
                customPalette.colors.Remove(color);
                _customitem.Initialize(customPalette, true);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(customPalette);
#else
                customPalette.SaveToJSON(JSONRuntimePath);
#endif
            }
        }
    }
}