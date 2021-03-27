using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class ScaledAffection
    {
        struct ScaleEntity
        {
            public Transform transform;
            public ScaledObjectData data;
        }

        private Vector2 _scale;
        private List<ScaleEntity> _entity;

        public ScaledAffection(Transform characterTransform, List<ScaledObjectData> data)
        {
            _scale = new Vector2(BodySegmentScaler.DefaultScale, BodySegmentScaler.DefaultScale);
            _entity = new List<ScaleEntity>();
            foreach (ScaledObjectData d in data)
            {
                Transform trans = characterTransform.Find(d.objectPath);
                _entity.Add(new ScaleEntity() { transform = trans, data = d });
            }
        }

        public void SetScale(Vector2 scale)
        {
            if (_entity == null)
                return;

            _scale = new Vector2(scale.x, scale.y);

            foreach (ScaleEntity e in _entity)
            {
                Vector3 newscale = new Vector3(e.data.defScale.x, e.data.defScale.y, 1.0f);

                if (_scale.x > 0.5f)
                    newscale.x = Mathf.Lerp(e.data.defScale.x, e.data.maxScale.x, (_scale.x - 0.5f) * 2.0f);
                else if (_scale.x < 0.5f)
                    newscale.x = Mathf.Lerp(e.data.minScale.x, e.data.defScale.x, _scale.x * 2.0f);

                if (_scale.y > 0.5f)
                    newscale.y = Mathf.Lerp(e.data.defScale.y, e.data.maxScale.y, (_scale.y - 0.5f) * 2.0f);
                else if (_scale.y < 0.5f)
                    newscale.y = Mathf.Lerp(e.data.minScale.y, e.data.defScale.y, _scale.y * 2.0f);

                e.transform.localScale = newscale;
            }
        }

        public Vector2 GetScale()
        {
            return _scale;
        }
    }
}