using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreator2D
{
    public class PositionedAffection
    {
        struct PosEntity
        {
            public PositionedObjectData data;
            public Vector2 scale;
        }

        public Vector2 scale { get; private set; }

        private Transform _transform;
        private Dictionary<SegmentType, PosEntity> _entity;

        public PositionedAffection(Transform transform)
        {
            _transform = transform;
            _entity = new Dictionary<SegmentType, PosEntity>();
        }

        public void AddEntity(SegmentType segmentType, PositionedObjectData data)
        {
            if (_entity.ContainsKey(segmentType))
                return;

            _entity.Add(segmentType, new PosEntity() { data = data, scale = new Vector2(BodySegmentScaler.DefaultScale, BodySegmentScaler.DefaultScale) });
        }

        public void SetScale(SegmentType segmentType, Vector2 scale)
        {
            if (_entity == null || !_entity.ContainsKey(segmentType))
                return;

            PosEntity entity = _entity[segmentType];
            entity.scale = scale;
            _entity[segmentType] = entity;
            updateTransform();
        }

        public Vector2 GetScale(SegmentType segmentType)
        {
            if (_entity == null || !_entity.ContainsKey(segmentType))
                return Vector2.one;
            else
                return _entity[segmentType].scale;
        }

        private void updateTransform()
        {
            if (_transform == null || _entity == null || _entity.Count <= 0)
                return;

            Vector3 pos = _entity.First().Value.data.defaultPos;
            pos = new Vector3(pos.x, pos.y);
            foreach (SegmentType k in _entity.Keys)
            {
                float modx = 0;
                float mody = 0;
                if (_entity[k].data.scaleX)
                {
                    if (_entity[k].scale.x > 0.5f)
                    {
                        modx += Mathf.Lerp(_entity[k].data.defaultPos.x, _entity[k].data.maxPosX.x, (_entity[k].scale.x - 0.5f) * 2.0f) - _entity[k].data.defaultPos.x;
                        mody += Mathf.Lerp(_entity[k].data.defaultPos.y, _entity[k].data.maxPosX.y, (_entity[k].scale.x - 0.5f) * 2.0f) - _entity[k].data.defaultPos.y;
                    }
                    else if (_entity[k].scale.x < 0.5f)
                    {
                        modx += Mathf.Lerp(_entity[k].data.minPosX.x, _entity[k].data.defaultPos.x, _entity[k].scale.x * 2.0f) - _entity[k].data.defaultPos.x;
                        mody += Mathf.Lerp(_entity[k].data.minPosX.y, _entity[k].data.defaultPos.y, _entity[k].scale.x * 2.0f) - _entity[k].data.defaultPos.y;
                    }
                }

                if (_entity[k].data.scaleY)
                {
                    if (_entity[k].scale.y > 0.5f)
                    {
                        modx += Mathf.Lerp(_entity[k].data.defaultPos.x, _entity[k].data.maxPosY.x, (_entity[k].scale.y - 0.5f) * 2.0f) - _entity[k].data.defaultPos.x;
                        mody += Mathf.Lerp(_entity[k].data.defaultPos.y, _entity[k].data.maxPosY.y, (_entity[k].scale.y - 0.5f) * 2.0f) - _entity[k].data.defaultPos.y;
                    }
                    else if (_entity[k].scale.y < 0.5f)
                    {
                        modx += Mathf.Lerp(_entity[k].data.minPosY.x, _entity[k].data.defaultPos.x, _entity[k].scale.y * 2.0f) - _entity[k].data.defaultPos.x;
                        mody += Mathf.Lerp(_entity[k].data.minPosY.y, _entity[k].data.defaultPos.y, _entity[k].scale.y * 2.0f) - _entity[k].data.defaultPos.y;
                    }
                }

                pos.x += modx;
                pos.y += mody;
            }
            pos.z = _transform.localPosition.z;
            _transform.localPosition = pos;
        }
    }
}