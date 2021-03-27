using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class Weapon : Part
    {
        /// <summary>
        /// Weapon category.
        /// </summary>
        [Tooltip("Weapon category")]
        public WeaponCategory weaponCategory;

        /// <summary>
        /// Muzzle flash position.
        /// </summary>
        [Tooltip("Muzzle flash position")]
        public Vector3 muzzlePosition;
    }
}