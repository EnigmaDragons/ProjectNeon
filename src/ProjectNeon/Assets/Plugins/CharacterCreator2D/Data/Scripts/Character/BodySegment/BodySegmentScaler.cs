using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    [Serializable]
    public class BodySegmentScaler
    {
        /// <summary>
        /// Minimum scale allowed for each segment.
        /// </summary>
        public const float MinScale = 0.0f;

        /// <summary>
        /// Maximum scale allowed for each segment.
        /// </summary>
        public const float MaxScale = 1.0f;

        /// <summary>
        /// Default scale of a segment.
        /// </summary>
        public const float DefaultScale = 0.5f;


        [SerializeField]
        [BodySlider(0.0f, 1.0f, true)]
        private Vector2 _headScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, true)]
        private Vector2 _earScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, false)]
        private Vector2 _neckScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, false)]
        private Vector2 _bodyScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, false)]
        private Vector2 _upperArmScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, false)]
        private Vector2 _lowerArmScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, true)]
        private Vector2 _handScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, false)]
        private Vector2 _upperleg = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, false)]
        private Vector2 _lowerLegScale = new Vector2(0.5f, 0.5f);

        [SerializeField]
        [BodySlider(0.0f, 1.0f, true)]
        private Vector2 _footScale = new Vector2(0.5f, 0.5f);

        [NonSerialized]
        private Dictionary<SegmentType, SegmentData> _segmentdatas;
        [NonSerialized]
        private Dictionary<SegmentType, ScaledAffection> _scaledaffections;
        [NonSerialized]
        private Dictionary<string, PositionedAffection> _posaffections;

        /// <summary>
        /// Initialize this BodySegmentScaler
        /// </summary>
        /// <param name="character">Character to be scaled.</param>
        public void Initialize(CharacterViewer character)
        {
            //..init _segmentdatas
            _segmentdatas = new Dictionary<SegmentType, SegmentData>();
            List<SegmentData> segmentdata = getSegmentDatas(character);
            if (segmentdata == null)
                return;
            foreach (SegmentData d in segmentdata)
                _segmentdatas.Add(d.segmentType, d);
            //init _segmentdatas..

            _scaledaffections = new Dictionary<SegmentType, ScaledAffection>();
            _posaffections = new Dictionary<string, PositionedAffection>();
            foreach (SegmentType t in _segmentdatas.Keys)
            {
                //..init scaled affections
                _scaledaffections.Add(t, new ScaledAffection(character.transform, _segmentdatas[t].scaledObjects));
                //init scaled affections..

                //..init positioned affections
                foreach (PositionedObjectData d in _segmentdatas[t].positionedObjects)
                {
                    PositionedAffection taff = null;
                    if (_posaffections.ContainsKey(d.objectPath))
                        taff = _posaffections[d.objectPath];
                    else
                    {
                        Transform trans = character.transform.Find(d.objectPath);
                        taff = new PositionedAffection(trans);
                        _posaffections.Add(d.objectPath, taff);
                    }

                    taff.AddEntity(t, d);
                }
                //init positioned affections..
            }

            foreach (SegmentType t in _segmentdatas.Keys)
                SetScale(t, GetScale(t));
        }

        /// <summary>
        /// Set selected segment's scale.
        /// </summary>
        /// <param name="segmentType">Selected segment</param>
        /// <param name="scale">Scale value ranged from 0 to 1. Different values will be forced to equals if the selected segment is symmetrical.</param>
        public void SetScale(SegmentType segmentType, Vector2 scale)
        {
            if (!_segmentdatas.ContainsKey(segmentType))
            {
                return;
            }

            Vector2 prevscale = GetScale(segmentType);
            //..check symmetrical
            if (_segmentdatas[segmentType].symmetrical)
            {
                if (prevscale.x != scale.x && prevscale.y == scale.y)
                    scale.y = scale.x;
                else if (prevscale.x == scale.x && prevscale.y != scale.y)
                    scale.x = scale.y;
                else if (prevscale.x != scale.x && prevscale.y != scale.y)
                    scale.x = scale.y = ((scale.x + scale.y) * 0.5f);
            }
            //check symmetrical..

            setScale(segmentType, scale);

            //..update scaled affection
            _scaledaffections[segmentType].SetScale(scale);
            //update scaled affection..

            //..update positioned affection
            foreach (PositionedObjectData d in _segmentdatas[segmentType].positionedObjects)
            {
                if (!_posaffections.ContainsKey(d.objectPath))
                    continue;

                _posaffections[d.objectPath].SetScale(segmentType, scale);
            }
            //update positioned affection..
        }

        private void setScale(SegmentType segmentType, Vector2 scale)
        {
            switch (segmentType)
            {
                case SegmentType.Body:
                    _bodyScale = scale;
                    break;
                case SegmentType.Ear:
                    _earScale = scale;
                    break;
                case SegmentType.Foot:
                    _footScale = scale;
                    break;
                case SegmentType.Hand:
                    _handScale = scale;
                    break;
                case SegmentType.Head:
                    _headScale = scale;
                    break;
                case SegmentType.LowerArm:
                    _lowerArmScale = scale;
                    break;
                case SegmentType.LowerLeg:
                    _lowerLegScale = scale;
                    break;
                case SegmentType.Neck:
                    _neckScale = scale;
                    break;
                case SegmentType.UpperArm:
                    _upperArmScale = scale;
                    break;
                case SegmentType.UpperLeg:
                    _upperleg = scale;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get selected segment's scale.
        /// </summary>
        /// <param name="segmentType">Selected segment.</param>
        /// <returns>Scale value of the selected segment.</returns>
        public Vector2 GetScale(SegmentType segmentType)
        {
            switch (segmentType)
            {
                case SegmentType.Body:
                    return _bodyScale;
                case SegmentType.Ear:
                    return _earScale;
                case SegmentType.Foot:
                    return _footScale;
                case SegmentType.Hand:
                    return _handScale;
                case SegmentType.Head:
                    return _headScale;
                case SegmentType.LowerArm:
                    return _lowerArmScale;
                case SegmentType.LowerLeg:
                    return _lowerLegScale;
                case SegmentType.Neck:
                    return _neckScale;
                case SegmentType.UpperArm:
                    return _upperArmScale;
                case SegmentType.UpperLeg:
                    return _upperleg;
                default:
                    return new Vector2(DefaultScale, DefaultScale);
            }
        }

        private List<SegmentData> getSegmentDatas(CharacterViewer character)
        {
            try
            {
                return character.setupData.bodyTypeData.Find(d => d.bodyType == character.bodyType).segmentDatas;
            }
            catch
            {
                return null;
            }
        }
    }
}