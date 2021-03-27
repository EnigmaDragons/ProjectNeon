using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class PartReferer : ScriptableObject
    {
#if UNITY_EDITOR || CC2D_RES

        /// <summary>
        /// Part's main texture.
        /// </summary>
        [Tooltip("Part's main texture")]
        public Texture2D texture;

        /// <summary>
        /// Part's color mask.
        /// </summary>
        [Tooltip("Part's color mask")]
        public Texture2D colorMask;

        /// <summary>
        /// List of sprite used by this Part.
        /// </summary>
        [Tooltip("List of sprite used by this Part")]
        public List<Sprite> sprites;

        private void OnDisable()
        {
            texture = null;
            colorMask = null;
            if (sprites != null)
            {
                sprites.Clear();
            }
        }

#endif
    }
}
