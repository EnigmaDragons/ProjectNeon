using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class BodyTypeData
    {
        public BodyType bodyType;
        public string packageName;
        public Transform prefab;
        public List<SegmentData> segmentDatas;
    }
}