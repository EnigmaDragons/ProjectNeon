using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D.UI
{
    public class ColorPalette : ScriptableObject
    {
        /// <summary>
        /// Display name of this ColorPalette.
        /// </summary>
        [Tooltip("Display name of this ColorPalette")]
        public string paletteName;

        /// <summary>
        /// Set of colors owned by this ColorPalette.
        /// </summary>
        [Tooltip("Set of colors owned by this ColorPalette")]
        public List<Color> colors;

        /// <summary>
        /// Save this ColorPalette as a JSON file.
        /// </summary>
        /// <param name="path">Desired path of the JSON file.</param>
        /// <returns>'true' on success, otherwise returns 'false'.</returns>
        public bool SaveToJSON(string path)
        {
            try
            {
                string msg = JsonUtility.ToJson(this);
                File.WriteAllText(path, msg);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Initialize this ColorPalette according to the data from a given path.
        /// </summary>
        /// <param name="path">The path of the JSON file.</param>
        /// <returns>'true' on success, otherwise returns 'false'.</returns>
        public bool LoadFromJSON(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                ColorPalette tpalette = JsonUtility.FromJson<ColorPalette>(path);
                this.paletteName = tpalette.paletteName;
                this.colors = new List<Color>();
                foreach (Color c in tpalette.colors)
                    this.colors.Add(c);
                return true;
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
        }
    }
}