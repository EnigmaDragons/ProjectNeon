using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class SegmentData
    {
        public SegmentType segmentType = SegmentType.Body;
        public bool symmetrical;
        public List<ScaledObjectData> scaledObjects = new List<ScaledObjectData>();
        public List<PositionedObjectData> positionedObjects = new List<PositionedObjectData>();
    }
}