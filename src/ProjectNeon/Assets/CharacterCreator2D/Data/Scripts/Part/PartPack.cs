using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class PartPack
    {
        /// <summary>
        /// PartPack's PartCategory.
        /// </summary>
        [Tooltip("PartPack's PartCategory")]
        public PartCategory category;

        /// <summary>
        /// List of Parts grouped by this PartPacks.
        /// </summary>
        [Tooltip("List of Parts grouped by this PartPacks")]
        public List<Part> parts;

        public PartPack()
        {
            this.category = PartCategory.Armor;
            this.parts = new List<Part>();
        }
    }
}