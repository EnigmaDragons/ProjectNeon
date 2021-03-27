using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class PartTemplate : ScriptableObject
    {
        /// <summary>
        /// PartTemplate's PartCategory.
        /// </summary>
        [Tooltip("PartTemplate's PartCategory")]
        [ReadOnly]
        public PartCategory category;

        /// <summary>
        /// PartTemplate's main texture.
        /// </summary>
        [Tooltip("PartTemplate's main texture")]
        [ReadOnly]
        public Texture2D texture;

        /// <summary>
        /// List of sprite names managed by this PartTemplate.
        /// </summary>
        [Tooltip("List of sprite names managed by this PartTemplate")]
        [ReadOnly]
        public List<string> spriteNames;

        public override string ToString()
        {
            return this.name;
        }
    }
}