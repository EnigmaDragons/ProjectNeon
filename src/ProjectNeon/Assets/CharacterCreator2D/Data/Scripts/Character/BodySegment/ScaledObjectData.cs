using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class ScaledObjectData
    {
        public string objectPath = "";
        public Vector2 defScale = Vector2.one;
        public Vector2 minScale = new Vector2(0.8f, 0.8f);
        public Vector2 maxScale = new Vector2(1.2f, 1.2f);
    }
}