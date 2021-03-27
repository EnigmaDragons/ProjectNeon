using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D
{
    public class CharacterEntity : MonoBehaviour
    {
        private string _fullpath;
        private Vector3 _locpos, _locscale;
        private Quaternion _locrot;

        public void Detach(CharacterViewer character)
        {
            _fullpath = getFullpath(character);
            _locpos = transform.localPosition;
            _locrot = transform.localRotation;
            _locscale = transform.localScale;

            if (!hasParentEntity())
                this.transform.SetParent(null);
        }

        public void Attach(CharacterViewer character)
        {
            if (hasParentEntity())
                return;

            Transform targetparent = character.transform.Find(_fullpath);
            if (character.transform.Find(string.Format("{0}/{1}", _fullpath, this.name)))
            {
                Destroy(this.gameObject);
            }
            transform.SetParent(targetparent);
            transform.localPosition = _locpos;
            transform.localRotation = _locrot;
            transform.localScale = _locscale;
        }

        private bool hasParentEntity()
        {
            List<CharacterEntity> entities = new List<CharacterEntity>(GetComponentsInParent<CharacterEntity>(true));
            return entities.Find(x => x.gameObject != this.gameObject);
        }

        private string getFullpath(CharacterViewer character)
        {
            Transform target = transform.parent;
            string val = target.name;
            while (target.parent != null && target.parent.gameObject != character.gameObject)
            {
                val = string.Format("{0}/{1}", target.parent.name, val);
                target = target.parent;
            }
            return val;
        }
    }
}