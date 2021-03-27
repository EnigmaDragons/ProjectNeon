using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class PaletteItem : MonoBehaviour
    {
        /// <summary>
        /// ColorItem's template.
        /// </summary>
        [Tooltip("ColorItem's template")]
        public ColorItem tColorItem;

        /// <summary>
        /// ColorPalette loaded by this PaletteItem.
        /// </summary>
        [Tooltip("ColorPalette object represents this PaletteItem")]
        [ReadOnly]
        public ColorPalette colorPalette;
        
        /// <summary>
        /// Initialize PaletteItem with a given ColorPalette to load.
        /// </summary>
        /// <param name="colorPalette">ColorPalette to be loaded this PaletteItem.</param>
        public void Initialize(ColorPalette colorPalette)
        {
            Initialize(colorPalette, false);
        }

        /// <summary>
        /// Initialize PaletteItem with a given ColorPalette to load.
        /// </summary>
        /// <param name="colorPalette">ColorPalette to be loaded this PaletteItem.</param>
        /// <param name="editable">Is PaletteItem editable?</param>
        public void Initialize(ColorPalette colorPalette, bool editable)
        {
            this.colorPalette = colorPalette;
            clearItems();
            spawnItems(editable);
        }

        private void clearItems()
        {
            ColorItem[] items = this.transform.GetComponentsInChildren<ColorItem>(true);
            for (int i = 0; i < items.Length; i++)
            {
                GameObject go = items[i].gameObject;
                items[i] = null;
                Destroy(go);
            }
        }

        private void spawnItems(bool editable)
        {
            foreach (Color c in colorPalette.colors)
            {
                ColorItem item = Instantiate<ColorItem>(tColorItem, this.transform);
                item.Initialize(c);
                item.editable = editable;
            }
        }
    }
}