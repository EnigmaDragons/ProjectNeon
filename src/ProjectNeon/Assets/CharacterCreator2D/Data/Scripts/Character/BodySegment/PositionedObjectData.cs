using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class PositionedObjectData
    {
        public string objectPath;
        public Vector2 defaultPos;
        public bool scaleX;
        public Vector2 minPosX;
        public Vector2 maxPosX;
        public bool scaleY;
        public Vector2 minPosY;
        public Vector2 maxPosY;
    }
}